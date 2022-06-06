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
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;

namespace CppOptimizationTool
{
    public partial class Form2 : Form
    {
        private Dictionary<string, ParserFuncDescriptor> _tableData;
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
        private string _targetFuncOut
        {
            get => this.targetFuncOut.Text;
            set {
                this.targetFuncOut.Visible = true;
                this.targetFuncOut.Text = $"Прогнозируемое суммарное время выделения динамической памяти будет равно {value}";
            }
        }


        private string _pathToFile { get; set; }

        public Form2(
            ref Dictionary<string, ParserFuncDescriptor> tableData
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
            foreach (KeyValuePair<string, ParserFuncDescriptor> item in _tableData)
            {
                foreach (string arrname in item.Value.ArraysList)
                {
                    this.dataGridView1.Rows.Add(
                        item.Key,
                        arrname,
                        null,
                        null,
                        item.Value.CallsCount
                    );
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            List<IndexedItem<double>> K = new List<IndexedItem<double>>();
            List<Item> values = new List<Item>();

            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow item = this.dataGridView1.Rows[i];
                int.TryParse(item.Cells["w_i_min"].Value.ToString(), out int wi_min);
                int.TryParse(item.Cells["w_i_max"].Value.ToString(), out int wi_max);
                int.TryParse(item.Cells["n_i"].Value.ToString(), out int ni);
                double ki = (double)ni / (2 * S * (wi_max - wi_min));
                K.Add(
                    new IndexedItem<double>
                    {
                        Idx = i,
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

            if (dataGridView1.Columns["Ri"] == null)
            {
                this.dataGridView1.Columns.Add("Ri", "Ri");
            }
            this.dataGridView1.Columns["Ri"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

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
            _pathToFile = $"{folderDlg.SelectedPath}\\optimized-{filename}";

            (double, List<int>) data = await Replacer.MakeReplaces(
                values,
                K,
                _pathToFile,
                TotalRam,
                _tableData,
                Connector._parser,
                Connector.selectedFileContent
            );
            
            for (int i = 0; i < data.Item2.Count; i++)
            {
                dataGridView1.Rows[i].Cells["Ri"].Value = data.Item2[i];
            }
            _targetFuncOut = Math.Round(data.Item1, 3).ToString();
            this.showOptimizedCode.Visible = true;
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

        private void openOptimizedFileButtonHandler(object sender, EventArgs e)
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "CMD.exe",
                    Arguments = $"/c notepad {_pathToFile}",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            }.Start();
        }
    }

    public class Item
    {
        public int Wi_min { get; set; }
        public int Wi_max { get; set; }
        public int Ni { get; set; }
    }
}
