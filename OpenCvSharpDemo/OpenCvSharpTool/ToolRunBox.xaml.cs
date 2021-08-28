using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace OpenCvSharpTool
{
    /// <summary>
    /// ToolRunBox.xaml 的交互逻辑
    /// </summary>
    public partial class ToolRunBox : UserControl
    {
        public ToolRunBox()
        {
            InitializeComponent();
        }

        public WriteableBitmap OutputBitmap
        {
            get { return (WriteableBitmap)GetValue(OutputBitmapProperty); }
            set { SetValue(OutputBitmapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutputBitmap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutputBitmapProperty =
            DependencyProperty.Register(nameof(OutputBitmap), typeof(WriteableBitmap), typeof(ToolRunBox));


        private void ButtonRun_OnClick(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource clt = new CancellationTokenSource();
            CancellationToken cls = clt.Token;
            ButtonRun.IsEnabled = false;


            Task.Run(() =>
           {
               while (true)
               {
                   DispatcherHelper.Delay(10);
                   try
                   {
                       Dispatcher.Invoke(delegate
                       {
                           ToolBase tool = ToolStackPanel.Children[ToolStackPanel.Children.Count - 1] as ToolBase;

                           if (tool != null && tool.OutputParams.ContainsKey("OutputImage"))
                           {
                               OutputBitmap = tool.OutputParams["OutputImage"] as WriteableBitmap;
                           }
                       });
                   }
                   catch (Exception exception)
                   {
                       Console.WriteLine(exception);
                       throw;
                   }

                   if (cls.IsCancellationRequested)
                   {
                       return;
                   }
               }
           }, cls);

            if (CheckBoxContinue.IsChecked==true)
            {
                while (CheckBoxContinue.IsChecked==true)
                {
                    DispatcherHelper.Delay(1000);
                    foreach (var child in ToolStackPanel.Children)
                    {
                        ToolBase tool = child as ToolBase;
                        tool?.Run();
                    }
                }
                clt.Cancel();
                ButtonRun.IsEnabled = true;
            }
            else
            {
                foreach (var child in ToolStackPanel.Children)
                {
                    ToolBase tool = child as ToolBase;
                    tool?.Run();
                }

                clt.Cancel();
                ButtonRun.IsEnabled = true;
            }
        }
    }
}
