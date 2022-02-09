using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Mvvm;
using Prism.Regions;

namespace PrismModuleA.ViewModels
{
    public class ModuleAViewAViewModel : BindableBase, IConfirmNavigationRequest
    {
        public ModuleAViewAViewModel()
        {

        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(); }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("Title"))
            {
                Title = navigationContext.Parameters.GetValue<string>("Title");
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            bool result = !(MessageBox.Show("确认跳转", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No);

            continuationCallback(result);
        }
    }
}
