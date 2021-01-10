using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Attributes.Process;

namespace Core.Models.Process
{
    public class ProcessInfoContainer
    {
        public ProcessConfigAttribute Conf { get; set; }

        public Func<bool> NeedToCreateFunc { get; set; }

        public DateTime LastTimeStart { get; set; }
    }
}
