using System;
using System.Collections.Generic;
using System.IO;
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

namespace AsyncProgramModel
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

        private string[] fileNames =
        {
            "file01.txt",
            "file02.txt",
            "file03.txt",
            "file04.txt",
            "file05.txt",
            "file06.txt",
            "file07.txt",
            "file08.txt",
            "file09.txt",
            "file10.txt",
            "file11.txt",
            "file12.txt",
            "file13.txt",
            "file14.txt",
            "file15.txt",
            "file16.txt"
        };
        private List<Task<string>> fileTasks;
        private const int BUFFER_SIZE = 0x2000;

        private Task<string> ReadAllTextAsync(string path)
        {
            FileInfo info = new FileInfo(path);
            if (!info.Exists)
            {
                throw new FileNotFoundException($"{path} does not exist.", path);
            }

            byte[] data = new byte[info.Length];

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, true);

            Task<int> task = Task<int>.Factory.FromAsync(stream.BeginRead, stream.EndRead, data, 0, data.Length, null, TaskCreationOptions.None);

            return task.ContinueWith(t =>
            {
                stream.Close();
                Console.WriteLine($"One task has read {t.Result} bytes from {stream.Name}");
                return (t.Result > 0) ? new UTF8Encoding().GetString(data) : "";
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
        private void butConCatTextFiles_Click(object sender, RoutedEventArgs e)
        {
            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();


            pgbFiles.Value = 0;
            pgbFiles.Maximum = fileNames.Length;

            Task.Factory.StartNew(() =>
            {
                fileTasks = new List<Task<string>>(fileNames.Length);
                foreach (string name in fileNames)
                {
                    var readTask = ReadAllTextAsync(Environment.CurrentDirectory + $"/File/{name}");

                    readTask.ContinueWith(t =>
                    {
                        AdvanceProgressBar();
                    }, uiScheduler);
                    fileTasks.Add(readTask);
                }

                //Task.WaitAll(fileTasks.ToArray());

                //foreach( Task<string> fileTask in fileTasks)
                //{
                //    Console.WriteLine($"{fileTask.Result}");
                //}

                var buildTask = Task.Factory.ContinueWhenAll(
                    fileTasks.ToArray(),
                    antecedentTasks =>
                    {
                        Task.WaitAll(antecedentTasks);
                        var sb = new StringBuilder();

                        foreach (Task<string> item in antecedentTasks)
                        {
                            sb.Append(item.Result);
                            sb.Append('\n');
                        }

                        return sb.ToString();
                    });

                buildTask.ContinueWith(antecedentTask =>
                {
                    txtAllText.Text = antecedentTask.Result;

                }, uiScheduler);
            });
        }

        private void AdvanceProgressBar()
        {
            pgbFiles.Value++;
        }
    }
}
