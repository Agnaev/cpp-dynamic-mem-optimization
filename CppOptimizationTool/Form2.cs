using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;

namespace CppOptimizationTool
{
    public partial class Form2 : Form
    {
        private Dictionary<string, (int, List<string>)> _tableData;
        public int S { get; set; } = 1;
        public long TotalRam
        {
            get
            {
                string value = this.ram.Text;
                if (string.IsNullOrEmpty(value))
                {
                    return (long)new ComputerInfo().TotalPhysicalMemory;
                }
                long.TryParse(this.ram.Text, out long ram);
                return ram;
            }
            set
            {
                this.ram.Text = value.ToString();
            }
        }
        private bool CanOptimize
        {
            get => this.do_optimize.Enabled;
            set => this.do_optimize.Enabled = value;
        }

        public Form2(
            ref Dictionary<string, (int, List<string>)> tableData
        )
        {
            InitializeComponent();
            this._tableData = tableData;
            this.ram.Text = new ComputerInfo().TotalPhysicalMemory.ToString();

            this.addRamTooltip(
                this.ramTooltip,
                new Control[] {
                    this.ramLabel,
                    this.ram
                }
            );
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("funcs: " + String.Join(" ", _funcCallsDict.Select(item => item.Key + " " + item.Value)));
            foreach (KeyValuePair<string, (int, List<string>)> item in _tableData)
            {
                foreach (string arrname in item.Value.Item2)
                {
                    this.dataGridView1.Rows.Add(
                        item.Key,
                        arrname,
                        null,
                        null,
                        item.Value.Item1
                    );
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            double Q = 0;
            List<(int, double)> K = new List<(int, double)>();
            var values = new List<Item>();
            //foreach (DataGridViewRow item in this.dataGridView1.Rows)
            int i = 0;
            for (i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow item = this.dataGridView1.Rows[i];
                int.TryParse(item.Cells["w_i_min"].Value.ToString(), out int wi_min);
                int.TryParse(item.Cells["w_i_max"].Value.ToString(), out int wi_max);
                int.TryParse(item.Cells["n_i"].Value.ToString(), out int ni);
                double ki = (double)ni / (2 * S * (wi_max - wi_min));
                K.Add((i, ki));
                values.Add(new Item
                {
                    Wi_min = wi_min,
                    Wi_max = wi_max,
                    Ni = ni,
                });
                Q += ni * Math.Pow(wi_max, 2) / (S * (wi_max - wi_min));
            }
            List<int> Z = K
                .OrderByDescending(
                    el => el.Item2
                )
                .Select(
                    el => el.Item1
                )
                .ToList();

            long v_curr = TotalRam;
            Item currentItem;
            List<int> U = new int[Z.Count].ToList();

            i = 0;
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

            //MessageBox.Show(String.Join(" ", R) + " " + v_curr);

            var replaceData = new Dictionary<string, List<(string, int)>>();
            i = 0;
            foreach (KeyValuePair<string, (int, List<string>)> item in _tableData)
            {
                if (!replaceData.ContainsKey(item.Key))
                {
                    replaceData.Add(item.Key, new List<(string, int)>());
                }
                foreach (string arrName in item.Value.Item2)
                {
                    int ri = R[i++];
                    if (ri != 0)
                    {
                        replaceData[item.Key].Add((arrName, ri));
                    }
                }
            }

            //var allocations = new Dictionary<string, List<(string, int)>>();

            //string fnName;
            //List<(string, int)> arrays = new List<(string, int)>();
            //for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            //{
            //    DataGridViewRow row = this.dataGridView1.Rows[i];

            //    fnName = row.Cells["func_name"].Value as string;
            //    allocations.Add(fnName, arrays);
            //    arrays.Clear();
            //}

            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            if (folderDlg.ShowDialog(this) != DialogResult.OK)
            {
                MessageBox.Show(
                    "Вы должны указать папку.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            string filename = Path.GetFileName(
               Connector.pathToSelectedFile
            );
            string pathToFile = $"{folderDlg.SelectedPath}\\optimized-{filename}";
            FileWriter writer = new FileWriter(
                pathToFile,
                FileMode.Create
            );

            foreach (string line in Connector._parser.Parse(
                Connector.selectedFileContent,
                replaceData
            ))
            {
                await writer.appendAsync(line);
            }

            writer.Close();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CanOptimize = this.dataGridView1.Rows.Count != 1;
            foreach (DataGridViewRow item in this.dataGridView1.Rows)
            {
                if (
                    string.IsNullOrEmpty(item.Cells["w_i_min"].Value as string) ||
                    string.IsNullOrEmpty(item.Cells["w_i_max"].Value as string)
                )
                {
                    CanOptimize = false;
                    break;
                }
            }
        }

        public void addRamTooltip(ToolTip tooltip, Control[] elements)
        {
            foreach (Control ctrl in elements)
            {
                tooltip.SetToolTip(ctrl, "Объем оперативной памяти");
            }
        }

        private struct Item
        {
            public int Wi_min { get; set; }
            public int Wi_max { get; set; }
            public int Ni { get; set; }
        }
    }
}
