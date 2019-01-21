using common_models;
using message_types.commands;
using message_types.events;

namespace database_registry.service.messages
{
    public class DatabaseRegisteredMessage : DatabaseRegistered
    {
        public Schema Schema { get; set; }

        public RegisterDatabase Database { get; set; }
    }
}
