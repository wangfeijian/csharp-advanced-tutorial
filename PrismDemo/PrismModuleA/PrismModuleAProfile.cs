using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Modularity;
using PrismModuleA.ViewModels;
using PrismModuleA.Views;

namespace PrismModuleA
{
    public class PrismModuleAProfile:IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ModuleAViewA, ModuleAViewAViewModel>();
            containerRegistry.RegisterDialog<DialogView, DialogViewViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}
