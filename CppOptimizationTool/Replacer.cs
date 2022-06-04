using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace CppOptimizationTool
{
    internal static class Replacer
    {
        public static async Task<(double, List<int>)> MakeReplaces(
            List<Item> values,
            List<IndexedItem<double>> K,
            string pathToFile,
            long v_curr,
            Dictionary<string, ParserFuncDescriptor> tableData,
            int S = 1
        )
        {
            double Q = values.Select(
                el => (el.Ni * Math.Pow(el.Wi_max, 2)) / (2 * S * (el.Wi_max - el.Wi_min))
            ).Sum();

            List<int> Z = K
                .OrderByDescending(
                    el => el.Value
                )
                .Select(
                    el => el.Idx
                )
                .ToList();

            Item currentItem;
            List<int> U = new int[Z.Count].ToList();

            int i = 0;
            for (int j = 0; j < Z.Count; j++)
            {
                i = Z[j];
                currentItem = values[i];
                if (v_curr < currentItem.Wi_min)
                {
                    U[i] = currentItem.Wi_min;
                    v_curr = Math.Max(v_curr - U[i], 0);
                }
                if (v_curr < currentItem.Wi_max)
                {
                    U[i] = (int)v_curr;
                }
                else
                {
                    U[i] = currentItem.Wi_max;
                }
                v_curr -= U[i];
            }

            List<int> R = new List<int>();
            for (i = 0; i < U.Count; i++)
            {
                currentItem = values[i];
                int ui = U[i];
                R.Add(Math.Sign(ui - currentItem.Wi_min) * ui);
            }

            double targetFuncValue = Q - K.Select(
                el => el.Value * Math.Pow(R[el.Idx], 2)
            ).Sum();

            var replaceData = new Dictionary<string, List<(string, int)>>();
            i = 0;
            foreach (KeyValuePair<string, ParserFuncDescriptor> item in tableData)
            {
                if (!replaceData.ContainsKey(item.Key))
                {
                    replaceData.Add(item.Key, new List<(string, int)>());
                }
                foreach (string arrName in item.Value.ArraysList)
                {
                    int ri = R[i++];
                    if (ri != 0)
                    {
                        replaceData[item.Key].Add((arrName, ri));
                    }
                }
            }

            FileWriter writer = new FileWriter(
                pathToFile,
                FileMode.OpenOrCreate
            );

            foreach (string line in Connector._parser.Parse(
                Connector.selectedFileContent,
                replaceData
            ))
            {
                if (false == await writer.appendAsync(line))
                {
                    writer.ClearFile();
                    MessageBox.Show(
                        "Произошла ошибка при записи в файл.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return (.0, null);
                }
            }

            writer.Close();

            return (targetFuncValue, R);
        }
    }
}
