using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Hubs
{
    public class IdObject<T>
    {
        public long Id { get; set; }

        public T Obj { get; set; }
    }
}
