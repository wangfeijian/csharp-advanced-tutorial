using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using PrismRegionDemo.Views;

namespace PrismRegionDemo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private IRegionNavigationJournal _journal;
        public DelegateCommand<string> OpenCommand { get;}
        public DelegateCommand<string> DialogCommand { get;}
        public DelegateCommand BackCommand { get;}
        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            // 使用Prism区域需要注入IRegionManager
            _regionManager = regionManager;
            _dialogService = dialogService;
            OpenCommand = new DelegateCommand<string>(OpenForRegion);
            BackCommand = new DelegateCommand(Back);
            DialogCommand = new DelegateCommand<string>(OpenDialog);

            //OpenCommand = new DelegateCommand<string>(Open);
        }

        private void OpenDialog(string obj)
        {
            DialogParameters keys = new DialogParameters();
            keys.Add("Value","Hello");
            _dialogService.ShowDialog(obj,keys, callback =>
            {
                if (callback.Result == ButtonResult.OK)
                {
                    string result = callback.Parameters.GetValue<string>("Value");
                }
            });
        }

        private void Back()
        {
            if(_journal.CanGoBack)
                _journal.GoBack();
        }

        #region 未使用Prism区域
        private object _body;

        public object Body
        {
            get { return _body; }
            set { _body = value; RaisePropertyChanged(); }
        }

        private void Open(string obj)
        {
            switch (obj)
            {
                case "ViewA":
                    Body = new ViewA();
                    break;
                case "ViewB":
                    Body = new ViewB();
                    break;
                case "ViewC":
                    Body = new ViewC();
                    break;
            }
        }
        #endregion

        #region 使用Prism区域
        // 删除前台ContentControl中 Content="{Binding Body}"
        // 添加prism:RegionManager.RegionName="RegionBody"
        private void OpenForRegion(string obj)
        {
            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add("Title","Hello");
            _regionManager.Regions["RegionBody"].RequestNavigate(obj, callback =>
            {
                if ((bool)callback.Result)
                {
                    _journal = callback.Context.NavigationService.Journal;
                }
            },navigationParameters);
        }
        #endregion
    }
}
