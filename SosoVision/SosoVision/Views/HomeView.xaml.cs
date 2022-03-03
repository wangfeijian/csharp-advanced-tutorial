using System.Windows;
using System.Windows.Controls;
using Prism.Events;
using Prism.Ioc;
using SosoVision.Common;
using SosoVision.ViewModels;
using SosoVisionTool.Tools;

namespace SosoVision.Views
{
    /// <summary>
    /// HomeView.xaml 的交互逻辑
    /// </summary>
    public partial class HomeView : UserControl
    {
        private readonly IConfigureService _configureService;

        public HomeView(IConfigureService configureService)
        {
            _configureService = configureService;
            InitializeComponent();
            InitHomeView();
        }

        private void InitHomeView()
        {
            int row = _configureService.SerializationData.Row;
            int col = _configureService.SerializationData.Col;

            for (int i = 0; i < row; i++)
            {
                RowDefinition rowDefinition = new RowDefinition() { Height = new GridLength(300) };
                ShowGrid.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0; i < col; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                ShowGrid.ColumnDefinitions.Add(columnDefinition);
            }

            foreach (var item in _configureService.SerializationData.ProcedureParams)
            {
                var view = ContainerLocator.Container.Resolve<VisionProcessView>(item.Name);
                var viewModel = view.DataContext as VisionProcessViewModel;

                GroupBox groupBox = new GroupBox() { Header = item.Name, Margin = new Thickness(4, 2, 8, 16) };
                ImageDisplay.ImageDisplayWindow imageDisplayWindow = new ImageDisplay.ImageDisplayWindow() { CameraColor = "#673ab7", RegionColor = "green" };
                imageDisplayWindow.EventAggregator.GetEvent<HObjectEvent>().Subscribe((obj) =>
                 {
                     if (!string.IsNullOrWhiteSpace(obj.Result))
                     {
                         imageDisplayWindow.DisplayMessage = "";
                         var results = obj.Result.Split(',');
                         var messages = obj.ShowMessage.Split(',');
                         string[] tempStr = new string[results.Length];

                         if (results.Length != messages.Length || results.Length <= 0)
                             return;

                         for (int i = 0; i < results.Length; i++)
                         {
                             tempStr[i] = $"{messages[i]}: {results[i]}";
                         }

                         string temp = string.Join("\n", tempStr);
                         if (obj.Result.Contains("999"))
                         {
                            imageDisplayWindow.MessageColor = "red";
                             imageDisplayWindow.DisplayMessage = $"NG\n{temp}";
                         }
                         else
                         {
                             imageDisplayWindow.MessageColor = "green";
                             imageDisplayWindow.DisplayMessage = $"OK\n{temp}";
                         }
                         return;
                     }
                     else
                     {
                         imageDisplayWindow.DisplayMessage = string.Empty;
                     }

                     imageDisplayWindow.DisplayImage = obj.Image;
                     imageDisplayWindow.DisplayRegion = obj.Region;
                 }, ThreadOption.UIThread, true,
                             companySymbol => companySymbol.VisionStep == item.Name);

                groupBox.Content = imageDisplayWindow;

                ShowGrid.Children.Add(groupBox);
                Grid.SetColumn(groupBox, item.ShowIdCol);
                Grid.SetRow(groupBox, item.ShowIdRow);
            }
        }
    }
}
