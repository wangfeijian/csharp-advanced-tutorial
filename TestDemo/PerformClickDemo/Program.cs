using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerformClickDemo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length>1)
            {
                MessageBox.Show(args[0], args[1], MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Application.Run(new FormMain());
        }
    }
}
