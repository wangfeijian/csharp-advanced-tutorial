using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PerformClickDemo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnShowHello_Click(object sender, EventArgs e)
        {
            string strHello = "你好啊";
            string strInfo = "提示";
            MessageBox.Show(strHello, strInfo, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
        }

        private void btnShowError_Click(object sender, EventArgs e)
        {
            string strError = "错误";
            string strInfo = "提示";
            MessageBox.Show(strError, strInfo, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //btnShowHello.PerformClick();
            // 在窗口加载时自动触发按钮的Click事件
            btnShowError.PerformClick();
        }
        
    }
}
