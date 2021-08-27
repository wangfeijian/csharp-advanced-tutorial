using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Win32;

namespace OpenCvSharpTool
{
    /// <summary>
    /// ImageAcquisitionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageAcquisitionWindow : Window
    {
        public ImageAcquisitionTool ImageGrabTool { get; set; }

        public ImageAcquisitionWindow(ImageAcquisitionTool toolBase)
        {
            InitializeComponent();
            ImageGrabTool = toolBase;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                TreeViewItem inputItem = null, outputItem = null;
                TreeView tree = ImageGrabTool.Content as TreeView;

                if (tree != null)
                    foreach (TreeViewItem treeItem in tree.Items)
                    {
                        foreach (TreeViewItem item in treeItem.Items)
                        {
                            if (item.Header.ToString() == "输入")
                            {
                                inputItem = item;
                            }
                            else if (item.Header.ToString() == "输出")
                            {
                                outputItem = item;
                            }
                        }
                    }

                InitParams();

                if (ImageGrabTool.InputParams != null)
                {
                    foreach (var runToolInputParam in ImageGrabTool.InputParams)
                    {
                        TreeViewItem treeView = new TreeViewItem
                        {
                            Header = runToolInputParam.First().Key,
                            ToolTip = runToolInputParam.First().Value
                        };
                        inputItem?.Items.Add(treeView);
                    }
                }

                if (ImageGrabTool.OutputParams != null)
                {
                    foreach (var runToolOutputParam in ImageGrabTool.OutputParams)
                    {
                        TreeViewItem treeView = new TreeViewItem
                        {
                            Header = runToolOutputParam.First().Key,
                            ToolTip = runToolOutputParam.First().Value
                        };
                        outputItem?.Items.Add(treeView);
                    }
                }

                MessageBox.Show("配置完成");
                TestGroup.IsEnabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        /// <summary>
        /// 初始化输入参数，根据类型
        /// </summary>
        private void InitParams()
        {
            switch (ImageGrabTool.CurrenType)
            {
                case ImageGrabType.CaptureFile:
                    ImageGrabTool.InputParams = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object> {{nameof(ImageGrabTool.FileName), ImageGrabTool.FileName}},
                        new Dictionary<string, object> {{nameof(ImageGrabTool.IsColorImage), ImageGrabTool.IsColorImage}},
                        new Dictionary<string, object> {{nameof(ImageGrabTool.IsVideo), ImageGrabTool.IsVideo}},
                    };
                    break;
                case ImageGrabType.CaptureDir:
                    ImageGrabTool.InputParams = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object> {{nameof(ImageGrabTool.DirName), ImageGrabTool.DirName}},
                        new Dictionary<string, object> {{nameof(ImageGrabTool.IsColorImage), ImageGrabTool.IsColorImage}},
                    };
                    break;
                case ImageGrabType.CaptureCamera:
                    ImageGrabTool.InputParams = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object> {{nameof(ImageGrabTool.CameraType), ImageGrabTool.CameraType}},
                        new Dictionary<string, object> {{nameof(ImageGrabTool.IsColorImage), ImageGrabTool.IsColorImage}},
                        new Dictionary<string, object> {{nameof(ImageGrabTool.IsContinuousAcquisition), ImageGrabTool.IsContinuousAcquisition}},
                    };
                    break;
            }
        }

        private void RadioButtonFile_OnClick(object sender, RoutedEventArgs e)
        {
            SelectImageStackPanel.IsEnabled = false;
            ButtonStartPlay.IsEnabled = false;
            ButtonStopPlay.IsEnabled = false;

            RadioButton radioButton = sender as RadioButton;
            int index = Convert.ToInt32(radioButton.Tag.ToString());

            ImageGrabTool.CurrenType = (ImageGrabType)index;

            switch (radioButton.Name)
            {
                case "RadioButtonFile":
                    GetFilePath();
                    ImageGrabTool.FileName = PathTextBox.Text;
                    break;
                case "RadioButtonDir":
                    GetFilePath(false);
                    ImageGrabTool.DirName = PathTextBox.Text;
                    break;
                case "RadioButtonCamera":
                    break;
            }
        }

        private void GetFilePath(bool flag = true)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "All files(*.*)|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                string dirName = fileName.Substring(0, fileName.LastIndexOf('\\'));
                PathTextBox.Text = flag ? fileName : dirName;
                return;
            }

            PathTextBox.Text = "";
        }

        private void RadioButtonColorImage_OnClick(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            ImageGrabTool.IsColorImage = radioButton.Name == "RadioButtonColorImage";
        }
    }
}
