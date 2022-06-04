using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace CppOptimizationTool
{
    public partial class Form1 : Form
    {
        private List<FilePathView> _sourceFilePathList
        {
            get => Connector._sourceFilePathList;
        }
        private Parser _parser
        {
            get => Connector._parser;
        }
        
        private string _selectedFileName
        {
            get => Connector.pathToSelectedFile;
            set
            {
                if (!File.Exists(value))
                {
                    return;
                }
                Connector.pathToSelectedFile = value;
                Connector.selectedFileContent = File.ReadAllLines(value);
                this.selectedFileContent.Text = File.ReadAllText(value);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _setOptimizeButtonEnabled();
            EnvDTE.DTE dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            string activeFilePath = dte.ActiveDocument.FullName;
            if (Path.GetExtension(activeFilePath) != ".cpp")
            {
                return;
            }
            this.addFiles(new string[] { activeFilePath });
            this.filesListbox.SelectedIndex = 0;
            this._selectedFileName = activeFilePath;
        }

        private void addFilesBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Выберите файлы с исходным кодом на C/C++",
                Filter = "Файлы C++ (*.cpp)|*.cpp|Файлы C (*.c)|*.c|Заголочные (*.h)|*.h|Любые файлы (*.*)|*.*"
            };
            fileDialog.ShowDialog(this);

            this.addFiles(fileDialog.FileNames);
            _setOptimizeButtonEnabled();
        }

        private void addFiles(string[] filePaths)
        {
            foreach (string filePath in filePaths)
            {
                if (_sourceFilePathList.Any(fp => fp.FileName == filePath || fp.PathToFile == filePath))
                {
                    continue;
                }
                _sourceFilePathList.Add(new FilePathView(filePath));
                filesListbox.Items.Add(Path.GetFileName(filePath));
            }
            _setOptimizeButtonEnabled();
        }

        private void removeFromListBtn_Click(object sender, EventArgs e)
        {
            if (filesListbox.SelectedIndex == -1)
            {
                return;
            }
            _sourceFilePathList.RemoveAt(filesListbox.SelectedIndex);
            filesListbox.Items.RemoveAt(filesListbox.SelectedIndex);
            selectedFileContent.Text = "";
            _setOptimizeButtonEnabled();
        }

        private void startAnalysisBtn_Click(object sender, EventArgs e)
        {
            if (_sourceFilePathList.Count() == 0)
            {
                MessageBox.Show(
                    "Список файлов исходных кодов пуст! Запуск оптимизации невозможен.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (selectedFileContent.Text == string.Empty)
            {
                MessageBox.Show(
                    "Контента для оптимизации нет.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error

                );
                return;
            }

            if (filesListbox.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Выберите файл для оптимизации",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            string[] lines = Connector.selectedFileContent;

            Dictionary<string, ParserFuncDescriptor> tabledata = _parser.GetArrays(lines);

            new Form2(
                ref tabledata
            ).ShowDialog();
        }

        private void filesListbox_Click(object sender, EventArgs e)
        {
            int fileIndex = filesListbox.SelectedIndex;
            if (fileIndex > -1)
            {
                string pathToFile = _sourceFilePathList[fileIndex].PathToFile;
                this._selectedFileName = pathToFile;
            }
            _setOptimizeButtonEnabled();
        }

        private void filesListbox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any())
            {
                this.addFiles(files);
            }
            _setOptimizeButtonEnabled();
        }

        private void filesListbox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
            _setOptimizeButtonEnabled();
        }

        private void _setOptimizeButtonEnabled()
        {
            this.startAnalysisBtn.Enabled = filesListbox.Items.Count > 0
                && filesListbox.SelectedIndex != -1
                && !string.IsNullOrEmpty(selectedFileContent.Text);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Connector._sourceFilePathList.Clear();
        }
    }

    class Connector
    {
        static internal List<FilePathView> _sourceFilePathList { get; } = new List<FilePathView>();
        static internal Parser _parser { get; } = new Parser();
        static internal string[] selectedFileContent { get; set; }
        static internal string pathToSelectedFile { get; set; }
    }

    class FilePathView
    {
        public string PathToFile { get; private set; }
        public string FileName
        {
            get
            {
                return Path.GetFileName(this.PathToFile);
            }
        }

        public FilePathView(string pathToFile)
        {
            this.PathToFile = pathToFile;
        }
    }
}
