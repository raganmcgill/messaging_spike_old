using common_models;
using message_types;

namespace database_registry.service.messages
{
    public class DatabaseRegisteredMessage : DatabaseRegistered
    {
        public Schema Schema { get; set; }

        public RegisterDatabase Database { get; set; }
    }
}
