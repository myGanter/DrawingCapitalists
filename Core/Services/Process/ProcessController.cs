using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Ninject;

using Core.Expansions;
using Core.Attributes.Process;
using Core.Models.Process;

namespace Core.Services.Process
{
    public class ProcessController
    {
        private readonly IKernel DI;

        private readonly Thread WorkThread;

        private readonly DBLogger Logger;

        private readonly Dictionary<Type, ProcessInfoContainer> TypeInfo;

        public ProcessController(IKernel di, DBLogger logger)
        {
            TypeInfo = new Dictionary<Type, ProcessInfoContainer>();

            DI = di;

            Logger = logger;

            WorkThread = new Thread(ThreadMethod) 
            { 
                IsBackground = true                
            };
        }

        public void ThreadMethod()
        {
            var singleProcs = TypeInfo.Where(x => x.Value.Conf.Mode == StartMode.Single);
            foreach(var i in singleProcs)
            {
                var proc = (IProcess)DI.Get(i.Key);

                proc.RunAsync().ContinueWith(x => proc.Dispose());
            } 
            
            for (; ; )
            {
                var nonSingleProcs = TypeInfo.Where(x => x.Value.Conf.Mode != StartMode.Single);

                foreach (var i in nonSingleProcs)
                {
                    try
                    {
                        var conf = i.Value;

                        if (conf.NeedToCreateFunc?.Invoke() == false)
                            continue;

                        var interval = DateTime.Now - conf.LastTimeStart;
                        if (interval > conf.Conf.GetTimeSpan)
                        {
                            var proc = (IProcess)DI.Get(i.Key);
                            proc.RunAsync().ContinueWith(x => proc.Dispose());

                            conf.LastTimeStart = DateTime.Now;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog(LogLevel.Error, i, e, (o, e) => $"Type = {o.Key.FullName}", "ProcessController");
                    }                                      
                }

                Thread.Sleep(16);
            }
        }

        public void StartProcessor()
        {
            Initialize();

            WorkThread.Start();
        }

        private void Initialize()
        {
            var procType = typeof(IProcess);

            var types = procType.GetAllTypes();

            var typesAttrs = types
                .Select(x => new { T = x, A = x.GetCustomAttribute<ProcessConfigAttribute>(), M = (Func<bool>)x.GetMethod("GetNeedToCreateFunc").Invoke(null, null) })
                .Where(x => x.A.IsNotNull());

            foreach (var i in typesAttrs)
            {
                var tConf = new ProcessInfoContainer() 
                {
                    Conf = i.A,
                    NeedToCreateFunc = i.M
                };

                TypeInfo.Add(i.T, tConf);
            }
        }
    }
}
