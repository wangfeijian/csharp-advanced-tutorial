using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Modularity;
using PrismModuleB.Views;

namespace PrismModuleB
{
    public class PrismModuleBProfile:IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ModuleBViewB>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}
