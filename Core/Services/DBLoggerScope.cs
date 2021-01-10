using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Services.DB;

namespace Core.Services
{
    public class DBLoggerScope : IDisposable
    {
        public event Action OnDispose;

        public void Dispose()
        {
            OnDispose?.Invoke();
        }
    }
}
