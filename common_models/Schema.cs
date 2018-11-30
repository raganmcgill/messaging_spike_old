using System.Collections.Generic;

namespace common_models
{
    public class Schema
    {
        public List<Table> Tables { get; set; }

        public Schema()
        {
            Tables = new List<Table>();
        }
    }
}