using System;

namespace message_types.commands
{
    public interface Heartbeat
    {
        string Name { get; }

        string Path { get; }

        DateTime DateTime { get; }
    }
}