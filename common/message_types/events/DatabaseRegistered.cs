using common_models;
using message_types.commands;

namespace message_types.events
{
    public interface DatabaseRegistered
    {
        Database Schema { get; }

        ConnectionDetails Database { get; }
    }
}