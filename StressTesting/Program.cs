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

            //return;
            //string file = "D:\\Диплом 2022\\TestDiplom\\examples\\example7.cpp";
            //string[] lines = File.ReadAllLines(file);

            //Parser parser = new Parser();
            //Stopwatch timer = new Stopwatch();

            //timer.Start();
            //Dictionary<string, ParserFuncDescriptor> tableData = parser.GetArrays(lines);
            //timer.Stop();
            //Console.WriteLine("Analiz time: " + timer.ElapsedMilliseconds);
            //int S = 1;


            //List<IndexedItem<double>> K = new List<IndexedItem<double>>();
            //List<Item> values = new List<Item>();

            //translateInputData(
            //    ref tableData,
            //    out K,
            //    out values
            //);

            //string outFile = "D:\\temp\\optimized.cpp";
            //long v_curr = 100L;

            //timer.Start();
            //var outData = Replacer.MakeReplaces(
            //    values,
            //    K,
            //    outFile,
            //    v_curr,
            //    tableData,
            //    parser,
            //    lines,
            //    S
            //);
            //timer.Stop();

            //Console.WriteLine("replaces time: " + timer.ElapsedMilliseconds);
            //Console.ReadKey();
        }

        static void Test()
        {
            SourceCodeGenerator sourceCodeGenerator = new SourceCodeGenerator();
            Parser parser = new Parser();
            Stopwatch timer;

            Dictionary<string, ParserFuncDescriptor> tableData;
            List<IndexedItem<double>> K;
            List<Item> values;

            string[] fileLines;
            string outFilePath;
            long v_curr = rand.Next(0, 250);
            const int S = 1;
            for (int i = 10; i < EXPERIMENTS_COUNT; i++)
            {
                fileLines = sourceCodeGenerator.createCppCode(i);
                WriteToFile(i, fileLines);

                timer = new Stopwatch();
                timer.Start();
                tableData = parser.GetArrays(fileLines);
                timer.Stop();

                Console.WriteLine("Analiz time: " + timer.ElapsedMilliseconds);

                translateInputData(
                    ref tableData,
                    out K,
                    out values
                );

                outFilePath = Path.Combine(EXPERIMENT_PATH, $"{i}.cpp");

                timer = new Stopwatch();
                timer.Start();
                try
                {
                    Replacer.MakeReplaces(
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
                Console.WriteLine("replaces time: " + timer.ElapsedMilliseconds);
            }
        }

        static void WriteToFile(int experimentNum, in string[] fileLines)
        {
            string filePath = Path.Join(EXPERIMENT_PATH, $"{experimentNum}.cpp");
            File.WriteAllLines(filePath, fileLines);
        }

        static void translateInputData(
            ref Dictionary<string, ParserFuncDescriptor> tableData,
            out List<IndexedItem<double>> K,
            out List<Item> values,
            int S = 1
        )
        {
            values = new List<Item>();
            K = new List<IndexedItem<double>>();
            int ni, wi_min, wi_max, i = 0;
            double ki;
            foreach (KeyValuePair<string, ParserFuncDescriptor> item in tableData)
            {
                foreach (string arrname in item.Value.ArraysList)
                {
                    wi_min = rand.Next(25, 100);
                    wi_max = wi_min + 25;
                    ni = item.Value.CallsCount;
                    ki = (double)ni / (2 * S * (wi_max - wi_min));
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
            "long float"
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
                $"  {allocName}[0] = {getDefaultValue(dataType)};\n" +
                $"  delete[] {allocName};\n" +
                $"}}\n\n";
        }

        (string, string) mainTemplateBuilder()
        {
            string headers = "#include <chrono>\n#include <iostream>\n";

            string timeMeasure = $"\nint main() {{" +
                $"auto start = std::chrono::high_reosolution_clock::now();\n" +
                string.Join("();\n", this._funcsList) +
                $"auto end = std::chrono::high_reosolution_clock::now();\n" +
                $"int durationTime = std::chrono::duration_cast<std::chrono::seconds>(end - start).count();\n" +
                $"std::cout << durationTime << std::endl;\n" +
                $"return 0;" +
                $"}}";
            return (headers, timeMeasure);
        }

        private dynamic getDefaultValue(string dataType)
        {
            if (dataType == "string")
            {
                return GetHashString(6);
            }
            if (dataType == "char")
            {
                return _chars[_rand.Next(_chars.Length)];
            }
            return 1;
        }
    }
}
