using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Attributes.Process;
using Core.Expansions;
using Core.Services.AppState;

namespace Core.Services.Process
{
    [ProcessConfig(Mode = StartMode.Restartable, IntervalMinutes = 15)]//IntervalMinutes = 5
    public class AppObjectsProcess : IProcess
    {
        protected readonly AppObjects Objects;

        public AppObjectsProcess(AppObjects objects)
        {
            Objects = objects;
        }

        public async Task RunAsync()
        {
            await Task.Run(async () => 
            {
                var deathObjs = Objects
                .Where(x => x.Value.IsEmpty())
                .ToList();

                foreach (var i in deathObjs)
                {
                    Objects.RemoveObject(i.Key);
                    await i.Value.NotifyDeath();
                }
            });
        }

        public void Dispose()
        { }

        public static Func<bool> GetNeedToCreateFunc()
        {
            return () => true;
        }
    }
}
