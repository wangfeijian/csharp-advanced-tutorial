using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoBuildConfig.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;

namespace AutoBuildConfig.View
{
    /// <summary>
    /// PointWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PointWindow : Window
    {
        public PointWindow()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            PointGrid.RowDefinitions.Clear();
            var data = SimpleIoc.Default.GetInstance<PointViewModel>();
            data.PropChangeEvent += Data_PropChangeEvent;
            for (int i = 0; i < data.StationPoints.Count; i++)
            {
                RowDefinition row = new RowDefinition();
                PointGrid.RowDefinitions.Add(row);
            }

            int j = 0;
            foreach (var station in data.StationPoints)
            {
                GroupBox groupBox = new GroupBox();
                TextBlock text = new TextBlock();
                text.Text = station.Name;
                groupBox.Header = text;
                text.Style = FindResource("TextBlockDataGridHeader") as Style;
                groupBox.Margin = new Thickness(2);
                Grid.SetRow(groupBox,j);
                PointGrid.Children.Add(groupBox);
                ScrollViewer scroll = new ScrollViewer();
                scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                groupBox.Content = scroll;
                StackPanel stack = new StackPanel();
                scroll.Content = stack;
                DataGrid dataGrid = new DataGrid();
                dataGrid.ItemsSource = data.StationPoints[j].PointInfos;
                dataGrid.Style = FindResource("DataGridStyle")as Style;
                dataGrid.Columns.Add(GetDataGridTextColumn("序号","Index"));
                dataGrid.Columns.Add(GetDataGridTextColumn("点位名称","Name"));
                dataGrid.Columns.Add(GetDataGridTextColumn("X位置","XPos"));
                dataGrid.Columns.Add(GetDataGridTextColumn("Y位置","YPos"));
                dataGrid.Columns.Add(GetDataGridTextColumn("Z位置","ZPos"));
                dataGrid.Columns.Add(GetDataGridTextColumn("U位置","UPos"));
                dataGrid.Columns.Add(GetDataGridTextColumn("A位置","APos"));
                dataGrid.Columns.Add(GetDataGridTextColumn("B位置","BPos"));
                dataGrid.Columns.Add(GetDataGridTextColumn("C位置","CPos"));
                dataGrid.Columns.Add(GetDataGridTextColumn("D位置","DPos"));
                stack.Children.Add(dataGrid);
                j++;
            }
        }

        private void Data_PropChangeEvent()
        {
            this.LoadConfig();
        }

        private DataGridTextColumn GetDataGridTextColumn(string name,string binding)
        {
            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = name;
            column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            column.Binding = new Binding(binding);
            column.ElementStyle = FindResource("DataGridContentStyle") as Style;
            column.HeaderStyle = FindResource("DataGridHeadStyle")as Style;

            return column;
        }

        private void Config_Close_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
