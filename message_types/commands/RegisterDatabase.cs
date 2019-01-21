namespace message_types.commands
{
    public interface RegisterDatabase
    {
        string Server { get; }
        string Database { get; }
        string User { get; }
        string Password { get; }
    }
}
