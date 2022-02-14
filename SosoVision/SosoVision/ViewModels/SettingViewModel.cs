using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Services.Dialogs;

namespace SosoVision.ViewModels
{
    public class SettingViewModel:IDialogAware
    {
        public SettingViewModel()
        {
            
        }
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public string Title { get; } = "视觉配置";
        public event Action<IDialogResult> RequestClose;
    }
}
