using common_models;
using message_types.commands;
using message_types.events;

namespace monitor.service.messages
{
    public class DatabaseRegisteredMessage : DatabaseRegistered
    {
        public Database Schema { get; set; }

        public ConnectionDetails Database { get; set; }
    }
}
