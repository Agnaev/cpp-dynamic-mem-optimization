using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppOptimizationTool
{
    public class IndexedItem<T>
    {
        public int Idx { get; set; }
        public T Value { get; set; }
    }
}
