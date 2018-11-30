using common_models;

namespace message_types
{
    public interface DatabaseRegistered
    {
        Schema Schema { get; }

        RegisterDatabase Database { get; }
    }
}