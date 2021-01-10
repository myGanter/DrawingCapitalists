using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using Core.Services.DB;
using Core.Attributes.Process;

namespace Core.Services.Process
{
    [ProcessConfig(Mode = StartMode.Restartable, IntervalSeconds = 5)]
    public class LogProcess : IProcess
    {
        private readonly AppDBContext Context;

        public LogProcess(AppDBContext context)
        {
            Context = context;
        }

        public async Task RunAsync()
        {
            var logs = DBLoggerQueue.GetAll();
            
            await Context.Logs.AddRangeAsync(logs);
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {          
            Context.Dispose();
        }

        public static Func<bool> GetNeedToCreateFunc()
        {
            return () => DBLoggerQueue.Count > 0;
        }
    }
}
