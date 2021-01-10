using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services.DI
{
    public class NinjectConfigure : NinjectModule
    {
        private readonly List<Action<NinjectModule>> ConfigTypeActions;

        public NinjectConfigure()
        {
            ConfigTypeActions = new List<Action<NinjectModule>>();
        }

        public override void Load()
        {         
            ConfigTypeActions
                .Where(x => x != null)
                .ToList()
                .ForEach(x => x(this));            
        }

        public void AddTypeConfig(Action<NinjectModule> clbk)
        {
            ConfigTypeActions.Add(clbk);
        }
    }
}
