using System;
using System.Diagnostics;
using System.IO;
using CppOptimizationTool;
using System.Collections.Generic;

namespace StressTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string file = "D:\\Диплом 2022\\TestDiplom\\examples\\example7.cpp";
            string[] lines = File.ReadAllLines(file);

            Parser parser = new Parser();
            Random rand = new Random();
            Stopwatch timer = new Stopwatch();

            timer.Start();
            Dictionary<string, ParserFuncDescriptor> tableData = parser.GetArrays(lines);
            timer.Stop();
            Console.WriteLine("Analiz time: " + timer.ElapsedMilliseconds);


            List<IndexedItem<double>> K = new List<IndexedItem<double>>();
            List<Item> values = new List<Item>();

            int S = 1;
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

            string outFile = "D:\\temp\\optimized.cpp";
            long v_curr = 100L;

            timer.Start();
            var outData = Replacer.MakeReplaces(
                values,
                K,
                outFile,
                v_curr,
                tableData,
                parser,
                lines,
                S
            );
            timer.Stop();

            Console.WriteLine("replaces time: " + timer.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
