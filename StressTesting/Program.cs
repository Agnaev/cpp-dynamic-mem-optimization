using System;
using System.Diagnostics;
using System.IO;
using CppOptimizationTool;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace StressTesting
{
    internal class Program
    {
        static string EXPERIMENT_PATH { get; } = "D:\\Диплом 2022\\experiment";
        static int EXPERIMENTS_COUNT { get; } = 100;
        
        static Random rand { get; } = new Random();
        
        
        static void Main(string[] args)
        {
            Test();
        }

        static void Test()
        {
            SourceCodeGenerator sourceCodeGenerator = new SourceCodeGenerator();
            Parser parser = new Parser();
            Stopwatch timer = new Stopwatch();

            Dictionary<string, ParserFuncDescriptor> tableData;
            List<IndexedItem<double>> K;
            List<Item> values;

            string[] fileLines;
            string outFilePath;
            long v_curr;
            const int S = 1;
            string analyzTime;
            string replaceTime;
            (double, List<int>) response = (.0, new List<int>());
            //for (int i = 10; i < 11; i++)
            //{
            fileLines = sourceCodeGenerator.createCppCode(100);
            WriteToFile(0, fileLines);

            timer.Start();
            tableData = parser.GetArrays(fileLines);
            timer.Stop();

            analyzTime = timer.Elapsed.ToString("mm\\:ss\\.ff");

            TranslateInputData(
                ref tableData,
                out K,
                out values,
                out v_curr
            );

            outFilePath = Path.Combine(EXPERIMENT_PATH, $"0.optimized.cpp");

            timer.Reset();
            timer.Start();
            try
            {
                response = Replacer.MakeReplaces(
                    values,
                    K,
                    outFilePath,
                    v_curr,
                    tableData,
                    parser,
                    fileLines,
                    S
                );
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            timer.Stop();
            replaceTime = timer.Elapsed.ToString("mm\\:ss\\.ff");
            //}
            Console.WriteLine(String.Join('\n', response.Item2));
            Console.WriteLine($"Analyz time: {analyzTime}\nReplace time: {replaceTime}");
        }

        static void WriteToFile(int experimentNum, in string[] fileLines)
        {
            string filePath = Path.Join(EXPERIMENT_PATH, $"{experimentNum}.cpp");
            File.WriteAllLines(filePath, fileLines);
        }

        static void TranslateInputData(
            ref Dictionary<string, ParserFuncDescriptor> tableData,
            out List<IndexedItem<double>> K,
            out List<Item> values,
            out long v_current,
            int S = 1
        )
        {
            v_current = 0;
            values = new List<Item>();
            K = new List<IndexedItem<double>>();
            int ni, wi_min, wi_max, i = 0;
            double ki;
            foreach (KeyValuePair<string, ParserFuncDescriptor> item in tableData)
            {
                foreach (string arrname in item.Value.ArraysList)
                {
                    wi_min = rand.Next(150, 200);
                    wi_max = wi_min + 25;
                    ni = item.Value.CallsCount; // rand.Next(1, 10);
                    ki = (double)ni / (2 * S * (wi_max - wi_min));
                    v_current += (wi_max + wi_min) / 2;
                    K.Add(
                        new IndexedItem<double>
                        {
                            Idx = i++,
                            Value = ki
                        }
                    );
                    values.Add(new Item
                    {
                        Wi_min = wi_min,
                        Wi_max = wi_max,
                        Ni = ni,
                    });
                }
            }
        }
    }

    internal class SourceCodeGenerator
    {
        private List<string> _funcsList { get; } = new List<string>();
        private Random _rand { get; } = new Random();

        private string _chars { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private List<string> dataTypes { get; } = new List<string>
        {
            "char",
            "int",
            "size_t",
            "bool",
            "float",
            "double",
            "short",
            "short int",
            "unsigned short int",
            "unsigned int",
            "long int",
            "unsigned long int",
            "unsigned long long int",
            "unsigned char",
        };

        private string GetHashString(int length)
        {
            return new string(
                Enumerable.Repeat(_chars, length)
                .Select(
                    s => s[_rand.Next(s.Length)]
                ).ToArray()
            );
        }

        /// <summary>
        /// Генератор C++ кода
        /// </summary>
        /// <param name="dynamicMemAllocationTimes">Количество выделений динамической памяти.</param>
        /// <returns></returns>
        public string[] createCppCode(int dynamicMemAllocationTimes)
        {
            string[] code = new string[dynamicMemAllocationTimes + 2];

            string dataType;
            int idx;
            for (int i = 0; i < dynamicMemAllocationTimes; i++)
            {
                idx = _rand.Next(0, dataTypes.Count);
                dataType = dataTypes[idx];
                code[i + 1] = templateFunctionBuilder(dataType);
            }
            (string, string) mainCode = mainTemplateBuilder();
            code[0] = mainCode.Item1;
            code[^1] = mainCode.Item2;

            return code;
        }

        private string templateFunctionBuilder(string dataType)
        {
            int sourceCodeSize = _rand.Next(50, 150);
            string hash = GetHashString(5);
            string name = dataType.Replace(" ", "_") + '_' + hash;
            string funcName = name + "_func";
            string allocName = name + "_alloc";
            this._funcsList.Add(funcName);

            return $"void {funcName} () {{\n" +
                $"  {dataType}* {allocName} = new {dataType}[{sourceCodeSize}];\n" +
                //$"  {allocName}[0] = {getDefaultValue(dataType)};\n" +
                $"  for (int i = 0; i < {sourceCodeSize}; ++i) {allocName}[i] = {getDefaultValue(dataType)};\n" +
                $"  delete[] {allocName};\n" +
                $"}}\n\n";
        }

        (string, string) mainTemplateBuilder()
        {
            string headers = "#include <chrono>\n#include <iostream>\n";

            string timeMeasure = $"\nint main() {{" +
                $"auto start = std::chrono::high_resolution_clock::now();\n" +
                string.Join("();\n", this._funcsList) +
                $"();\n" +
                $"auto end = std::chrono::high_resolution_clock::now();\n" +
                $"int durationTime = std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count();\n" +
                $"std::cout << durationTime << std::endl;\n" +
                $"return 0;" +
                $"}}";
            return (headers, timeMeasure);
        }

        private dynamic getDefaultValue(string dataType)
        {
            if (dataType == "string")
            {
                return $"\"{GetHashString(6)}\"";
            }
            if (dataType == "char")
            {
                return $"'{_chars[_rand.Next(_chars.Length)]}'";
            }
            return 1;
        }
    }
}
