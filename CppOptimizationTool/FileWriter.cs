using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CppOptimizationTool
{
    internal class FileWriter
    {
        private FileStream _file;
        public FileWriter(string filename, FileMode filemod)
        {
            _file = new FileStream(filename, filemod);
        }

        public async Task<bool> appendAsync(string line)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(line + '\n');
                await _file.WriteAsync(buffer, 0, buffer.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            this._file.Close();
        }
    }
}
