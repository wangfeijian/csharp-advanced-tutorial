using Prism.Commands;
using Prism.Ioc;
using SosoVisionCommonTool.Log;

namespace SosoVision.ViewModels
{
    public class HomeViewModel
    {
        private readonly ISosoLogManager _sosoLogManager;
        public HomeViewModel()
        {
            
            _sosoLogManager = ContainerLocator.Container.Resolve<ISosoLogManager>();
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
