using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using common_models;

namespace message_types
{
    public class SimpleCommand
    {
    public Guid CommandId { get; set; }
    public DateTime Timestamp { get; set; }
    }

    public class SimpleCommandResult
    {
        public Guid ResultId { get; set; }
        public Guid CommandId { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public short ResultCode { get; set; }
        public string ResultText { get; set; }
    }


    public interface IGetSuggestions
    {
        string Prefix { get; }
    }

    public interface IReturnSuggestions
    {
        DateTime Timestamp { get; }
        List<Column>  Suggestions { get; set; }
    }
}
