using System;
using message_types.commands;

namespace dashboard.ui.Controllers
{
    public class Beat : Heartbeat
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime DateTime { get; set; }
        
        public double Age
        {
            get
            {
                var diffInSeconds = (DateTime.Now - DateTime).TotalSeconds;
                return diffInSeconds;
            }
        }
    }
}