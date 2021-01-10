using Core.Services.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.Process
{
    public interface IProcess : IDisposable
    {
        public Task RunAsync();
    }
}
