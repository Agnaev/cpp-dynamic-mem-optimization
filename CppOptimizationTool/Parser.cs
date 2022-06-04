using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CppOptimizationTool
{
    internal class Parser
    {
        private Random _random;
        private Dictionary<string, string> _cacheNames;
        private Dictionary<string, string> _staticCaches;
        private Dictionary<int, string> _codeBlocks;
        private Dictionary<int, string> _allocationPlaceHashs;
        private Dictionary<string, (int, string, string)> _allocationLines;
        private Dictionary<string, int> _funcCallsCounter;

        public Parser()
        {
            _random = new Random();
            _cacheNames = new Dictionary<string, string>();
            _staticCaches = new Dictionary<string, string>();
            _codeBlocks = new Dictionary<int, string>();
            _allocationPlaceHashs = new Dictionary<int, string>();
            _allocationLines = new Dictionary<string, (int, string, string)>();
            _funcCallsCounter = new Dictionary<string, int>();
        }

        private string staticCachesListToString()
        {
            return string.Join(" ", _staticCaches.Select(
                item => item.Value
            ).ToArray());
        }

        public Dictionary<string, ParserFuncDescriptor>
        GetArrays(string[] lines)
        {
            int prevNesting = 0;
            var result = new Dictionary<string, ParserFuncDescriptor>();

            foreach (var (i, line, nesting, fn) in makeParse(lines))
            {
                if (
                    line.Contains("new ") &&
                    line.Contains("[") &&
                    line.Contains("]")
                )
                {
                    string name = getNameFromLine(_removePointSigns(line));
                    if (!result.ContainsKey(fn))
                    {
                        result.Add(
                            fn,
                            new ParserFuncDescriptor
                            {
                                ArraysList = new List<string>(),
                                CallsCount = 0
                            }
                        );
                    }
                    result[fn].ArraysList.Add(name);

                    if (_allocationPlaceHashs.ContainsKey(nesting))
                    {
                        _allocationPlaceHashs[nesting] = _codeBlocks[nesting];
                    }
                    else
                    {
                        _allocationPlaceHashs.Add(nesting, _codeBlocks[nesting]);
                    }
                    name = $"{_codeBlocks[nesting]}_{name}";
                }

                if (line.Contains("delete") && _codeBlocks[nesting] != _allocationPlaceHashs[nesting] && result.ContainsKey(fn))
                {
                    string name = getNameFromLine(_removePointSigns(line));
                    result[fn].ArraysList = result[fn].ArraysList.Where(
                        el => el != name
                    ).ToList();
                }

                prevNesting = nesting;
            }

            foreach (var item in _funcCallsCounter)
            {
                if (!result.ContainsKey(item.Key))
                {
                    continue;
                }
                result[item.Key].CallsCount = item.Value;
            }

            this.clear();
            return result;
        }

        public string[] Parse(
            string[] lines,
            Dictionary<string, List<(string, int)>> allocations
        )
        {
            int prevNesting = 0;
            string[] result = new string[lines.Length];

            int staticCachesInsertPosition = -1;

            foreach (var (i, _line, nesting, fnName) in this.makeParse(lines))
            {
                string line = _line;

                if (nesting == 1 && prevNesting == 0 && staticCachesInsertPosition == -1)
                {
                    staticCachesInsertPosition = i;
                }

                if (
                    line.Contains("new ") &&
                    line.Contains("[") &&
                    line.Contains("]") &&
                    allocations.ContainsKey(fnName)
                )
                {
                    string name = getNameFromLine(_removePointSigns(line));
                    var allocationData = allocations[fnName].Where(el => el.Item1 == name).ToList();

                    if (allocationData != null && allocationData.Count() > 0)
                    {
                        string replacedLine = parseOptimize(line, allocationData[0].Item2, nesting);
                        if (_allocationPlaceHashs.ContainsKey(nesting))
                        {
                            _allocationPlaceHashs[nesting] = _codeBlocks[nesting];
                        }
                        else
                        {
                            _allocationPlaceHashs.Add(nesting, _codeBlocks[nesting]);
                        }
                        name = $"{_codeBlocks[nesting]}_{name}";
                        _allocationLines.Add(name, (i, line, replacedLine));
                        line = replacedLine;
                    }
                }

                if (line.Contains("delete"))
                {
                    string name = getNameFromLine(_removePointSigns(line));
                    if (allocations[fnName].Any(el => el.Item1 == name))
                    {
                        if (_codeBlocks[nesting] == _allocationPlaceHashs[nesting])
                        {
                            line = parseDelete(line, nesting, prevNesting);
                        }
                        else
                        {
                            name = $"{_allocationPlaceHashs[nesting]}_{name}";
                            (int, string, string) allocationData = _allocationLines[name];
                            line = allocationData.Item2;
                            _cacheNames.Remove(name);
                            _staticCaches.Remove(name);
                        }
                    }
                }

                result[i] = line;
                prevNesting = nesting;
            }

            if (staticCachesInsertPosition != -1)
            {
                result[staticCachesInsertPosition] = $"{this.staticCachesListToString()} {lines[staticCachesInsertPosition]}";
            }
            this.clear();

            return result;
        }

        internal IEnumerable<(int, string, int, string)> makeParse(string[] lines)
        {
            int nesting = 0;
            bool insideMultilineCommentsNesting = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Contains("//"))
                {
                    line = cutOnelineCommentFromLine(line);
                }

                if (line.Contains("/*"))
                {
                    insideMultilineCommentsNesting = true;
                }
                else if (line.Contains("*/"))
                {
                    insideMultilineCommentsNesting = false;
                }

                if (insideMultilineCommentsNesting)
                {
                    continue;
                }

                (bool, string) funcDecl = checkForFunctionDeclRegex(line);
                if (funcDecl.Item1)
                {
                    if (!_funcCallsCounter.ContainsKey(funcDecl.Item2))
                    {
                        _funcCallsCounter.Add(funcDecl.Item2, 0);
                    }
                }
                (bool, string) funcCall = checkForFnCall(line);
                if (funcCall.Item1)
                {
                    if (_funcCallsCounter.ContainsKey(funcCall.Item2))
                    {
                        _funcCallsCounter[funcCall.Item2]++;
                    }
                }

                if (line.Contains("{"))
                {
                    ++nesting;
                    _codeBlocks.Add(
                        nesting,
                        getHash()
                    );
                }

                if (line.Contains("}"))
                {
                    _codeBlocks.Remove(nesting);
                    --nesting;
                }

                string currentFnName = string.IsNullOrEmpty(funcDecl.Item2) && _funcCallsCounter.Count > 0
                    ? _funcCallsCounter.Last().Key
                    : funcDecl.Item2;

                yield return (i, line, nesting, currentFnName);
            }
        }

        private string parseDelete(string line, int nesting, int prevNesting)
        {
            string searchMark = "[]";
            int start = line.IndexOf(searchMark);

            string dynamicMemName = line.Substring(
                start + searchMark.Length,
                line.Length - start - searchMark.Length - 1
            ).Trim();
            string key = $"{_codeBlocks[nesting]}_{dynamicMemName}";
            if (!_cacheNames.ContainsKey(key))
            {
                return line;
            }
            string replacedMemName = _cacheNames[key];
            string condition = $"if ({dynamicMemName} != {replacedMemName})";

            return $"{condition} delete[] {dynamicMemName};";
        }

        private string parseOptimize(string line, int U, int nesting)
        {
            int start = line.IndexOf("new ");
            int length = line.IndexOf(";") - start;
            string dynamicMemoryAllocation = line.Substring(
                start,
                length
            );
            string W_dynamicMemAllocSize = getDynamicMemoryAllocationSize(line);
            string dynamicMemType = getDynamicMemoryType(line);
            string initialPart = line.Substring(0, start);
            string cacheName = $"x_cache_{nesting}_{getHash(10)}";
            string arrName = $"{_codeBlocks[nesting]}_{_removePointSigns(getAllocatedMemoryName(initialPart))}";

            this._cacheNames.Add(arrName, cacheName);

            this._staticCaches.Add(
                arrName,
                $"static {dynamicMemType} {cacheName}[(size_t)((double){U}/(double)sizeof({dynamicMemType}))];\n"
            );
            return $"{initialPart}" + // {type} {variable} = 
                $"((size_t)((double){W_dynamicMemAllocSize} * (double)sizeof({dynamicMemType})) > {U}" + // ternary operator W > array length
                $" ? {dynamicMemoryAllocation}" + // ?
                $" : {cacheName}" + // :
                $");"; // end
        }


        internal (bool, string) checkForFunctionDeclRegex(string line)
        {
            line = line.Replace("*", "").Replace("&", "");
            Regex regex = new Regex(@"^\S+\s+(\S*?)\s{0,}\(.*\)\s*{?");
            try
            {
                Match res = regex.Match(line);
                if (res != null)
                {
                    string value = res.Groups?[1]?.Value;
                    return (value?.Length != 0, value);
                }
            }
            catch { }

            return (false, null);
        }

        internal (bool, string) checkForFnCall(string line)
        {
            line = line.Replace("*", "").Replace("&", "");
            Regex regex = new Regex(@"^(?:\w+\s+\w+\s+=)?\s*(\w+=?)\s{0,}\(.*\);?");
            try
            {
                Match res = regex.Match(line);
                if (res != null)
                {
                    string value = res.Groups?[1]?.Value;
                    return (value?.Length != 0, value);
                }
            }
            catch { }
            return (false, "");
        }

        private void clear()
        {
            this._cacheNames.Clear();
            this._staticCaches.Clear();
            this._codeBlocks.Clear();
            this._allocationPlaceHashs.Clear();
            this._allocationLines.Clear();
            this._funcCallsCounter.Clear();
        }

        private string cutOnelineCommentFromLine(string line)
        {
            if (line.Contains("//"))
            {
                int commentStartIndex = line.IndexOf("//");
                line = line.Substring(0, commentStartIndex);
            }
            return line;
        }

        private int parseNestingFromCacheName(string cacheName)
        {
            string nesting = cacheName.Substring(8).Split('_')[0];
            return Convert.ToInt32(nesting);
        }

        private string getDynamicMemoryAllocationSize(string line)
        {
            int start = line.LastIndexOf("[");
            int length = line.LastIndexOf("]") - start;
            return line.Substring(start + 1, length - 1);
        }

        private string getDynamicMemoryType(string line)
        {
            string searchString = "new ";
            int start = line.LastIndexOf(searchString);
            int length = line.LastIndexOf("[") - start - searchString.Length;
            return line.Substring(start + searchString.Length, length);
        }

        private string getAllocatedMemoryName(string line)
        {
            line = line.TrimEnd(
                new char[] { ' ', '=' }
            );
            int start = line.LastIndexOf(' ');
            return line.Substring(start + 1);
        }

        internal string getNameFromLine(string line)
        {
            Regex reg = new Regex(@"\s=.+");
            line = reg.Replace(
                line,
                ""
            );
            reg = new Regex(@"\S+;{0,1}$");
            string value = reg.Match(line).Groups[0].Value;
            return value.TrimEnd(
                new char[] { ' ', ';' }
            );
        }

        private string getHash(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(
                Enumerable.Repeat(chars, length)
                .Select(
                    s => s[_random.Next(s.Length)]).ToArray()
                );
        }

        internal string _removePointSigns(string line) => line.Replace("*", "").Replace("&", "");
    }

    public class ParserFuncDescriptor
    {
        public int CallsCount { get; set; }
        public List<string> ArraysList { get; set; }
    }
}
