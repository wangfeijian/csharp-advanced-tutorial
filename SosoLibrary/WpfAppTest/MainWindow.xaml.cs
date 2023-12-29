using Soso.Services;
using System.Windows;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SystemServices.Instance.SystemParameterChangedEvent += OnSystemParameterChangedEvent;
        }

        private void OnSystemParameterChangedEvent(string key, string oldValue, string newValue)
        {
            MessageBox.Show($"key:{key}, oldValue:{oldValue}, newValue:{newValue}");
        }
    }
}
