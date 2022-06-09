using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackgroundWorkerDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // 添加处理方法
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;

            // 添加通过事件传递参数，更新UI
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.ProgressChanged += _backgroundWorker_ProgressChanged;

            // 添加支持取消操作
            _backgroundWorker.WorkerSupportsCancellation = true;

            // 捕获异常
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show($"用户取消了操作");
                return;
            }

            if (e.Error != null)
            {
                Type errorType = e.Error.GetType();
                switch (errorType.Name)
                {
                    case "ArgumentNullException":
                    case "MyException":
                        //do something.
                        break;
                    default:
                        //do something.
                        break;
                }

                return;
            }

            MessageBox.Show($"计算已完成，结果为{e.Result}");
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progressBar.Value = e.ProgressPercentage;

            _label.Text = e.UserState.ToString();
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int sum = 0, endNum = 0;

            if (e.Argument != null)
            {
                endNum = (int)e.Argument;
            }

            for (int i = 0; i <= endNum; i++)
            {
                if (_backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(100);
                string message = $"当前进度：{i}%";
                _backgroundWorker.ReportProgress(i, message);

                sum += i;
            }

            e.Result = sum;
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            _startButton.Enabled = false;
            _stopButton.Enabled = true;
            _backgroundWorker.RunWorkerAsync(100);
        }

        private void _stopButton_Click(object sender, EventArgs e)
        {
            _stopButton.Enabled = false;
            _startButton.Enabled = true;
            _backgroundWorker.CancelAsync();
        }
    }
}
