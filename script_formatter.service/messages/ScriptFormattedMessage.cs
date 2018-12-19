using message_types.events;

namespace script_formatter.service.messages
{
    public class ScriptFormattedMessage:ScriptFormatted
    {
        public string FormattedScript { get; set; }
    }
}
