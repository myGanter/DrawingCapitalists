using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Attributes.Process
{
    public class ProcessConfigAttribute : Attribute
    {
        public StartMode Mode { get; set; }

        public uint IntervalMilliseconds { get; set; }

        public uint IntervalSeconds { get; set; }

        public uint IntervalMinutes { get; set; }

        public uint IntervalHours { get; set; }

        public uint IntervalDays { get; set; }

        public TimeSpan GetTimeSpan { get => new TimeSpan((int)IntervalDays, (int)IntervalHours, (int)IntervalMinutes, (int)IntervalSeconds, (int)IntervalMilliseconds); }
    }
}
