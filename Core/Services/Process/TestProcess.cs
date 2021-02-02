using Core.Attributes.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Expansions;
using System.Threading.Tasks;

namespace Core.Services.Process
{
    [ProcessConfig(Mode = StartMode.Restartable, IntervalMinutes = 5)]
    public class TestProcess : IProcess
    {
        private readonly DBLogger Logger;

        public TestProcess(DBLogger logger)
        {
            Logger = logger;
        }

        public void Dispose()
        { }

        public async Task RunAsync()
        {
            Logger.WriteLog(Microsoft.Extensions.Logging.LogLevel.Information, "Test", null, (o, e) => o, "Test process");
            await Task.FromResult(0);
        }

        public static Func<bool> GetNeedToCreateFunc()
        {
            return () => true;
        }
    }
}
