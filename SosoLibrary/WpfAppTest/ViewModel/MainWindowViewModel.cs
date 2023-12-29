using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soso.Services;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppTest.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private int _lang = 0;

        private string _paramName;
        public string ParamName
        {
            get => _paramName;
            set => SetProperty(ref _paramName, value);
        }

        private string _paramValue;

        public string ParamValue
        {
            get { return _paramValue; }
            set { SetProperty(ref _paramValue, value); }
        }

        public SystemServices Services { get; set; }
        public AsyncRelayCommand ChangeLanguage { get; }
        public AsyncRelayCommand<object> GetSystemParameCommand { get; }
        public AsyncRelayCommand<object> SetSystemParameCommand { get; }
        public MainWindowViewModel()
        {
            Services = SystemServices.Instance;

            ChangeLanguage = new AsyncRelayCommand(async () => { _lang = _lang == 0 ? 1 : 0; SystemServices.Instance.ChangeLanguage(_lang); });

            GetSystemParameCommand = new AsyncRelayCommand<object>(GetSystemParam);
            SetSystemParameCommand = new AsyncRelayCommand<object>(SetSystemParam);
        }

        private async Task SetSystemParam(object index)
        {
            try
            {
                switch (index.ToString())
                {
                    case "0":
                        bool b;
                        if (!bool.TryParse(ParamValue, out b))
                        {
                            b = false;
                        }
                        SystemServices.Instance.SetBoolSystemParam(ParamName, b);
                        break;
                    case "1":
                        int i;
                        if (!int.TryParse(ParamValue, out i))
                        {
                            i = 0;
                        }
                        SystemServices.Instance.SetIntSystemParam(ParamName, i);
                        break;
                    case "2":
                        double d;
                        if (!double.TryParse(ParamValue, out d))
                        {
                            d = 0;
                        }
                        SystemServices.Instance.SetDoubleSystemParam(ParamName, d);
                        break;
                    default:
                        SystemServices.Instance.SetStringSystemParam(ParamName, ParamValue);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async Task GetSystemParam(object index)
        {
            try
            {
                string result;
                switch (index)
                {
                    case "0":
                        result = SystemServices.Instance.GetBoolSystemParam(ParamName).ToString();
                        break;
                    case "1":
                        result = SystemServices.Instance.GetIntSystemParam(ParamName).ToString();
                        break;
                    case "2":
                        result = SystemServices.Instance.GetDoubleSystemParam(ParamName).ToString();
                        break;
                    default:
                        result = SystemServices.Instance.GetStringSystemParam(ParamName);
                        break;
                }
                MessageBox.Show(result);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
