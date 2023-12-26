using Autofac;
using Soso.Contract;

namespace WpfAppTest.ViewModel
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel => DIServices.Instance.Container.Resolve<MainWindowViewModel>();
    }
}
