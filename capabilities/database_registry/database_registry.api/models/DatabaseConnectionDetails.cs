using common_models;
using message_types.commands;

namespace database_registry.api.models
{
    public class DatabaseConnectionDetails
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class DTO : RegisterDatabase
    {
        public ConnectionDetails ConnectionDetails { get; set; }
    }
}
