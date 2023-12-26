using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soso.Services;

namespace WpfAppTest.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private int _lang = 0;
        public SystemServices Services { get; set; }
        public AsyncRelayCommand ChangeLanguage { get; }
        public MainWindowViewModel()
        {
            Services = SystemServices.Instance;

            ChangeLanguage = new AsyncRelayCommand(async () => { _lang = _lang == 0 ? 1 : 0; SystemServices.Instance.ChangeLanguage(_lang); });
        }

    }
}
