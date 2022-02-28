using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prism.DryIoc;
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
