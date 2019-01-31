using common_models;
using message_types.commands;

namespace message_types.events
{
    public interface SchemaChanged
    {
        RegisterDatabase Database { get; }

        Schema Schema { get; }
    }

    public interface UpdateSchema
    {
        Schema Schema { get; }
    }
}