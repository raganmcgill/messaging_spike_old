using common_models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace helpers
{
    public static class StorageHelper
    {
        public static void StoreDatabaseDefinition(ConnectionDetails connectionDetails, List<Table> tables, string name)
        {
            string path = $@"C:\dev\Stores\{name}\{connectionDetails.Server}\{connectionDetails.Database}";

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            StoreConnectionDetails(connectionDetails, path);

            StoreTables(tables, path);
        }

        private static void StoreConnectionDetails(ConnectionDetails connectionDetails, string path)
        {
            var filename = "connection.txt";

            var serilised = JsonConvert.SerializeObject(connectionDetails);

            WriteFile(path, filename, serilised);
        }

        private static void StoreTables(List<Table> tables, string subPath)
        {
            foreach (var table in tables)
            {
                var filename = $"{table.Name}.txt";

                var serilisedTable = JsonConvert.SerializeObject(table);

                WriteFile(subPath, filename, serilisedTable);
            }
        }

        private static void WriteFile(string path, string filename, string data)
        {
            var fullPath = Path.Combine(path, filename);

            File.WriteAllText(fullPath, data);
        }
    }
}
