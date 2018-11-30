namespace message_types
{
    public interface RegisterDatabase
    {
        string Server { get; }
        string Database { get; }
        string Username { get; }
        string Password { get; set; }
    }
}
