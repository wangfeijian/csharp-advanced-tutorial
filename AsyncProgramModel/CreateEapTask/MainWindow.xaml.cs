using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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

namespace CreateEapTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string[] fileAddresses =
        {
            @"http://media.wiley.com/product_data/excerpt/4X/04705022/047050224X.pdf",
            @"http://media.wiley.com/product_data/excerpt/4X/04705022/047050224X-1.pdf",
            @"http://media.wiley.com/product_data/excerpt/4X/04705022/047050224X-2.pdf"
        };

        private List<Task<string>> _downloadTasks;
        private CancellationTokenSource _cts;
        private CancellationToken _clt;

        private void CreateToken()
        {
            _cts = new CancellationTokenSource();
            _clt = _cts.Token;
        }

        private Task<string> DownloadFileInTask(Uri address, CancellationToken clt)
        {
            var tcs = new TaskCompletionSource<string>(address);

            string fileName = Environment.CurrentDirectory + "\\" + address.Segments[address.Segments.Length - 1];

            FileInfo info = new FileInfo(fileName);
            if(info.Exists)
            {
                tcs.TrySetException(new InvalidOperationException($"{fileName} already exists."));
                return tcs.Task;
            }

            var wc = new WebClient();
            clt.Register(
                () =>
                {
                    if(wc != null)
                    {
                        wc.CancelAsync();
                        tcs.TrySetCanceled();
                    }
                });

            AsyncCompletedEventHandler handler = null;

            handler = (hSender, hE) =>
            {
                if(hE.Error != null)
                {
                    tcs.TrySetException(hE.Error);
                }
                else if(hE.Cancelled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    tcs.TrySetResult(fileName);
                }
                wc.DownloadFileCompleted -= handler;
            };

            wc.DownloadFileCompleted += handler;

            try
            {
                wc.DownloadFileAsync(address, fileName);
            }
            catch (Exception ex)
            {
                wc.DownloadFileCompleted -= handler;
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }


        private void butDownloadFiles_Click(object sender, RoutedEventArgs e)
        {
            CreateToken();

            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            lstStatus.Items.Clear();

            butCancel.IsEnabled = true;

            Task.Factory.StartNew(() =>
            {
                _downloadTasks = new List<Task<string>>(fileAddresses.Length);

                foreach(var address in fileAddresses)
                {
                    var downloadTask = DownloadFileInTask(new Uri(address), _clt);

                    downloadTask.ContinueWith(
                        t =>
                        {
                            string line = "";

                            if(t.IsCanceled)
                            {
                                line = "Canceled.";
                            }
                            else if(t.IsFaulted)
                            {
                                foreach (Exception innerEx in t.Exception.InnerExceptions)
                                {
                                    line += innerEx.Message;
                                }
                            }
                            else
                            {
                                line = t.Result;
                            }

                            lstStatus.Items.Add(line);
                        }, uiScheduler);

                    _downloadTasks.Add(downloadTask);
                }

                var uiTF = new TaskFactory(uiScheduler);
                uiTF.ContinueWhenAll(
                    _downloadTasks.ToArray(),
                    antecedentTasks =>
                    {
                        butCancel.IsEnabled = false;

                        var completed = (from task in antecedentTasks
                                         where !(task.IsCanceled || task.IsFaulted)
                                         select task).Count();

                        if(completed == antecedentTasks.Length)
                        {
                            lstStatus.Items.Add("All Downloads completed!");
                        }
                        _downloadTasks.Clear();
                    });
            });
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }
    }
}
