using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace schema_scanner.service.models
{
    public class DatabaseConnectionDetails
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
    }
}
