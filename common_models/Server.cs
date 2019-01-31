using System.Collections.Generic;

namespace common_models
{
    public class Server
    {
        public string  Name { get; set; }

        public List<Database> Databases { get; set; }

        public Server()
        {
            Databases = new List<Database>();
        }
    }

    public class Database
    {
        public ConnectionDetails ConnectionDetails { get; set; }

        public string Name { get; set; }

        public List<Table> Tables { get; set; }

        public Database()
        {
            Tables = new List<Table>();
        }

    }
}
