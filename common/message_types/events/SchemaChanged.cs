using common_models;

namespace message_types.events
{
    public interface SchemaChanged
    {
        Database Database { get; }
    }
}