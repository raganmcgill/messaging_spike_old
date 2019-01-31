using System;

namespace message_types.commands
{
    public interface Heartbeat
    {
        DateTime DateTime { get; }

        string Component { get; }

    }
}