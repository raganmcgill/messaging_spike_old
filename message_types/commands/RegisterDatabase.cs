using common_models;

namespace message_types.commands
{
    public interface RegisterDatabase
    {
        ConnectionDetails ConnectionDetails { get; set; }
    }
}
