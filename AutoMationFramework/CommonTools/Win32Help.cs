using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonTools
{
    public struct LastInputInfo
    {
        /// <summary>
        /// The size of the structure, in bytes. This member must be set to sizeof(LastInputInfo).
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public int CbSize;
        /// <summary>
        /// The tick count when the last input event was received.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint DwTime;
    }

    public static class Win32Help
    {
        /// <summary>帮助类</summary>
        /// <summary>Hides the window and activates another window</summary>
        public const int SwHide = 0;
        /// <summary>
        /// Activates and displays a window.
        /// If the window is minimized or maximized,
        /// the system restores it to its original size and position.
        ///  An application should specify this flag when displaying the window for the first time.
        /// </summary>
        public const int SwShownormal = 1;
        /// <summary>
        /// Activates the window and displays it in its current size and position
        /// </summary>
        public const int SwShow = 5;
        /// <summary>
        /// Activates and displays the window.
        /// If the window is minimized or maximized,
        /// the system restores it to its original size and position.
        /// An application should specify this flag when restoring a minimized window
        /// </summary>
        public const int SwRestore = 9;

        /// <summary>调用windows API获取鼠标键盘空闲时间</summary>
        /// <param name="plii"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LastInputInfo plii);

        /// <summary>该函数设置由不同线程产生的窗口的显示状态</summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWindow函数的说明部分</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        /// <summary>
        ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。
        ///  系统给创建前台窗口的线程分配的权限稍高于其他线程。
        /// </summary>
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>.指定的窗口是否最小化</summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        /// <summary>获取鼠标键盘空闲时间</summary>
        /// <returns></returns>
        public static int GetIdleTick()
        {
            LastInputInfo plii = new LastInputInfo();
            plii.CbSize = Marshal.SizeOf(plii);
            if (!GetLastInputInfo(ref plii))
                return 0;
            return Environment.TickCount - (int)plii.DwTime;
        }

        /// <summary>字节数组转换为ASCII字符串</summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ByteToAsciiString(byte[] array)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in array)
                stringBuilder.Append(num.ToString("X2"));
            return stringBuilder.ToString();
        }

        /// <summary>字节数组转换为ASCII字符串</summary>
        /// <param name="array"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string ByteToAsciiString(byte[] array, int startIndex, int length, char split = ' ')
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = startIndex; index < length; ++index)
            {
                stringBuilder.Append(array[index].ToString("X2"));
                if (index < length - 1)
                    stringBuilder.Append(split);
            }
            return stringBuilder.ToString();
        }

        /// <summary>ASCII字符串转换为字节数组</summary>
        /// <param name="strAscii"></param>
        /// <returns></returns>
        public static byte[] AsciiStringToByte(string strAscii)
        {
            byte[] numArray = new byte[strAscii.Length / 2];
            for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = Convert.ToByte(strAscii.Substring(index * 2, 2), 16);
            return numArray;
        }

        /// <summary>ASCII字符串转换为字节数组</summary>
        /// <param name="strAscii"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static byte[] AsciiStringToByte(string strAscii, string split)
        {
            return AsciiStringToByte(strAscii.Replace(split, ""));
        }

        /// <summary>获取当前进程的实例</summary>
        /// <param name="bIgnorePath">是否忽略路径比较，如果忽略路径比较，只能运行一个实例；
        /// 反之同一个路径下的只能运行一个</param>
        /// <returns></returns>
        public static Process GetCurrentInstance(bool bIgnorePath = true)
        {
            Process currentProcess = Process.GetCurrentProcess();
            if (currentProcess.ProcessName.Contains("vshost"))
                return null;
            foreach (Process process in Process.GetProcessesByName(currentProcess.ProcessName))
            {
                if (currentProcess.MainModule != null && (process.Id != currentProcess.Id && (bIgnorePath || Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == currentProcess.MainModule.FileName)))
                    return process;
            }
            return null;
        }

        /// <summary>重启应用程序</summary>
        public static void RestartApplication()
        {
            Process.Start(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>启动应用程序</summary>
        /// <param name="strAppPath">应用程序完整路径</param>
        /// <param name="bOneInstance">只运行一个进程</param>
        /// <returns></returns>
        public static bool StartApplication(string strAppPath, bool bOneInstance = true)
        {
            if ((uint)Process.GetProcessesByName(GetFileTitle(strAppPath)).Length > 0U & bOneInstance)
                return true;
            return new Process()
            {
                StartInfo = {
                 FileName = strAppPath,
                 WorkingDirectory = new FileInfo(strAppPath).DirectoryName
                }
            }.Start();
        }

        /// <summary>应用程序是否运行</summary>
        /// <param name="strAppName">应用程序的名称，不包含路径和后缀</param>
        /// <returns></returns>
        public static bool IsAppRunning(string strAppName)
        {
            return (uint)Process.GetProcessesByName(strAppName).Length > 0U;
        }

        /// <summary>关闭进程</summary>
        /// <param name="strAppName">应用程序的名称，不包含路径和后缀</param>
        /// <returns></returns>
        public static void StopApplication(string strAppName)
        {
            foreach (Process process in Process.GetProcessesByName(strAppName))
                process.Kill();
        }

        /// <summary>根据文件路径获取文件标题</summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static string GetFileTitle(string strPath)
        {
            FileInfo fileInfo = new FileInfo(strPath);
            int length = fileInfo.Name.LastIndexOf('.');
            if (length > 0)
                return fileInfo.Name.Substring(0, length);
            return fileInfo.Name;
        }

        /// <summary>将一个实体类复制到另一个实体类</summary>
        /// <param name="objectsrc">源实体类</param>
        /// <param name="objectdest">复制到的实体类</param>
        /// <param name="excudeFields">不复制的属性</param>
        public static void EntityToEntity(object objectsrc, object objectdest, params string[] excudeFields)
        {
            Type type1 = objectsrc.GetType();
            Type type2 = objectdest.GetType();
            foreach (PropertyInfo property in type2.GetProperties())
            {
                PropertyInfo item = property;
                if (excudeFields.All(x => x.ToUpper() != item.Name))
                    item.SetValue(objectdest, type1.GetProperty(item.ToString().ToLower())?.GetValue(objectsrc, null), null);
            }
            foreach (FieldInfo field in type2.GetFields())
            {
                FieldInfo item = field;
                if (excudeFields.All(x => x.ToUpper() != item.Name))
                    item.SetValue(objectdest, type1.GetField(item.ToString().ToLower()).GetValue(objectsrc));
            }
        }

        /// <summary>转换为弧度</summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double ToRad(double degree)
        {
            return degree / 180.0 * Math.PI;
        }

        /// <summary>转换为角度</summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double ToDegree(double rad)
        {
            return rad / Math.PI * 180.0;
        }

        /// <summary>文件夹拷贝</summary>
        /// <param name="strSourceFolder">源文件夹</param>
        /// <param name="strDestFolder">目标文件夹</param>
        public static void CopyFolder(string strSourceFolder, string strDestFolder)
        {
            if (!Directory.Exists(strDestFolder))
                Directory.CreateDirectory(strDestFolder);
            foreach (string fileSystemEntry in Directory.GetFileSystemEntries(strSourceFolder))
            {
                FileInfo fileInfo = new FileInfo(fileSystemEntry);
                string str = Path.Combine(strDestFolder, fileInfo.Name);
                if ((uint)(fileInfo.Attributes & FileAttributes.Directory) > 0U)
                    CopyFolder(fileInfo.FullName, str);
                else
                    File.Copy(fileInfo.FullName, str, true);
            }
        }

        /// <summary>连接共享文件夹</summary>
        /// <param name="path">共享路径</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="error">输出错误</param>
        /// <returns></returns>
        public static bool ConnectShare(string path, string user, string password, out string error)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.StandardInput.WriteLine(
                $"net use {(object)path} /USER:{(object)user} {(object)password} /PERSISTENT:YES");
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();
            error = process.StandardError.ReadToEnd().Trim();
            return string.IsNullOrEmpty(error);
        }

        /// <summary>文本加密</summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string Encode(string strText)
        {
            byte num1 = (byte)new Random().Next(0, byte.MaxValue);
            byte[] bytes = Encoding.Default.GetBytes(strText);
            List<byte> byteList = new List<byte>();
            byteList.Add(num1);
            foreach (byte num2 in bytes)
                byteList.Add((byte)(num2 ^ (uint)num1));
            return ByteToAsciiString(byteList.ToArray());
        }

        /// <summary>字符串解密</summary>
        /// <param name="strEncode"></param>
        /// <returns></returns>
        public static string Decode(string strEncode)
        {
            byte[] numArray = AsciiStringToByte(strEncode);
            byte num = numArray[0];
            List<byte> byteList = new List<byte>();
            for (int index = 1; index < numArray.Length; ++index)
                byteList.Add((byte)(numArray[index] ^ (uint)num));
            return Encoding.Default.GetString(byteList.ToArray());
        }

        ///// <summary>把DataGridView里的数据导出到Excel表</summary>
        ///// <param name="grid"></param>
        //public static void ExportGridToExcel(DataGridView grid)
        //{
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    saveFileDialog.Filter = "CSV Files| *.csv";
        //    if (saveFileDialog.ShowDialog() != DialogResult.OK)
        //        return;
        //    using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false, Encoding.Default))
        //    {
        //        StringBuilder stringBuilder = new StringBuilder();
        //        for (int index = 0; index < grid.Columns.Count; ++index)
        //        {
        //            if (grid.Columns[index].Visible)
        //                stringBuilder.Append(grid.Columns[index].HeaderText.ToString().Trim() + ",");
        //        }
        //        stringBuilder.Append(Environment.NewLine);
        //        for (int index1 = 0; index1 < grid.Rows.Count - 1; ++index1)
        //        {
        //            MediaTypeNames.Application.DoEvents();
        //            for (int index2 = 0; index2 < grid.Columns.Count; ++index2)
        //            {
        //                if (grid.Columns[index2].Visible)
        //                    stringBuilder.Append(grid.Rows[index1].Cells[index2].Value.ToString().Trim() + ",");
        //            }
        //            stringBuilder.Append(Environment.NewLine);
        //        }
        //        streamWriter.Write(stringBuilder.ToString());
        //    }
        //}
    }
}
