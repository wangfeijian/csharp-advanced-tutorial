using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.DryIoc;
using Prism.Ioc;
using SosoVision.Common;
using SosoVision.Views;

namespace SosoVision.ViewModels
{
    public class HomeViewModel
    {
        private ISosoLogManager _sosoLogManager;
        public HomeViewModel(IContainerProvider containerProvider)
        {
            
            _sosoLogManager = containerProvider.Resolve<ISosoLogManager>();
            ShowLogCommand = new DelegateCommand<string>(str =>
            {
                switch (str)
                {
                    case "info":
                        _sosoLogManager.ShowLogInfo("测试info");
                        break;
                    case "warn":
                        _sosoLogManager.ShowLogWarning("测试warning");
                        break;
                    case "error":
                        _sosoLogManager.ShowLogError("测试error");
                        break;
                }
            });
        }

        public DelegateCommand<string> ShowLogCommand { get; }
    }
}
