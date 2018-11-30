using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common_models;
using message_types;

namespace schema_scanner.service.messages
{
    public class DatabaseRegisteredMessage : DatabaseRegistered
    {
        public Schema Schema { get; set; }

        public RegisterDatabase Database { get; set; }
    }
}
