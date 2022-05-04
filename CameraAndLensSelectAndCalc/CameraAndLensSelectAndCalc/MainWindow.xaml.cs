using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CameraAndLensSelectAndCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _index;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void LensTextLostFocus(object sender, RoutedEventArgs e)
        {
            var textObj = sender as TextBox;
            GetDataForLens(textObj);
        }


        private void LensChanged(object sender, KeyEventArgs e)
        {
            var textObj = sender as TextBox;

            if (e.Key == Key.Enter)
            {
                GetDataForLens(textObj);
            }
        }

        private void GetDataForLens(TextBox textObj)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
                viewModel.LensChange(textObj.Text);
        }

        private void CameraModelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_index < 2)
            {
                _index++;
                return;
            }

            LensTextLostFocus(LensTextBox, null);
        }

        private void FinalCalcKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            if (e.Key != Key.Enter)
            {
                return;
            }

            switch (textBox.Tag)
            {
                case "1":
                    CalcForWidth(textBox);
                    break;
                case "2":
                    CalcForHeight(textBox);
                    break;
                default:
                    CalcForTimes(textBox);
                    break;
            }
        }

        private void FinalCalcLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            switch (textBox.Tag)
            {
                case "1":
                    CalcForWidth(textBox);
                    break;
                case "2":
                    CalcForHeight(textBox);
                    break;
                default:
                    CalcForTimes(textBox);
                    break;
            }
        }

        private void CalcForWidth(TextBox textBox)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
                viewModel.FinalCalcForWidth(textBox.Text);
        }

        private void CalcForHeight(TextBox textBox)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
                viewModel.FinalCalcForHeight(textBox.Text);
        }

        private void CalcForTimes(TextBox textBox)
        {
            var viewModel = DataContext as MainWindowViewModel;
            if (viewModel != null)
                viewModel.FinalCalcForTimes(textBox.Text);
        }
    }
}
