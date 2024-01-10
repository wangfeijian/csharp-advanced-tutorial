using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soso.Services;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppTest.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private int _lang;

        private string _paramName;
        public string ParamName
        {
            get => _paramName;
            set => SetProperty(ref _paramName, value);
        }

        private string _paramValue;

        public string ParamValue
        {
            get => _paramValue;
            set => SetProperty(ref _paramValue, value);
        }

        public SystemServices Services { get; set; }
        public AsyncRelayCommand ChangeLanguage { get; }
        public AsyncRelayCommand<object> GetSystemParamCommand { get; }
        public AsyncRelayCommand<object> SetSystemParamCommand { get; }
        public MainWindowViewModel()
        {
            Services = SystemServices.Instance;

            ChangeLanguage = new AsyncRelayCommand(() => { _lang = _lang == 0 ? 1 : 0; SystemServices.Instance.ChangeLanguage(_lang);
                return Task.CompletedTask;
            });

            GetSystemParamCommand = new AsyncRelayCommand<object>(GetSystemParam);
            SetSystemParamCommand = new AsyncRelayCommand<object>(SetSystemParam);
        }

        private Task SetSystemParam(object index)
        {
            try
            {
                switch (index.ToString())
                {
                    case "0":
                        if (!bool.TryParse(ParamValue, out var b))
                        {
                            b = false;
                        }
                        SystemServices.Instance.SetBoolSystemParam(ParamName, b);
                        break;
                    case "1":
                        if (!int.TryParse(ParamValue, out var i))
                        {
                            i = 0;
                        }
                        SystemServices.Instance.SetIntSystemParam(ParamName, i);
                        break;
                    case "2":
                        if (!double.TryParse(ParamValue, out var d))
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

            return Task.CompletedTask;
        }
        private Task GetSystemParam(object index)
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
                        result = SystemServices.Instance.GetDoubleSystemParam(ParamName).ToString(CultureInfo.InvariantCulture);
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

            return Task.CompletedTask;
        }
    }
}
