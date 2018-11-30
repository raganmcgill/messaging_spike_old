using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common_models
{
    public class Column
    {
        public string Name { get; set; }
        public int OrdinalPosition { get; set; }
        public string Default { get; set; }
        public DataType DataType { get; set; }
    }
}
