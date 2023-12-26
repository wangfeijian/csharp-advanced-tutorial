#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.FileHelp
 * 唯一标识：b12c345e-f2be-46e4-ae23-0e4931dff2e8
 * 文件名：CsvHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/27/2023 10:57:20 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：王飞箭 wangfeijian 
 * 时间：6/29/2023 13:47
 * 修改说明：
 * 1、将报错信息修改为英文
 * 版本：V1.0.0
  * ----------------------------------------------------------------
 * 修改人：王飞箭 wangfeijian 
 * 时间：7/5/2023
 * 修改说明：
 * 1、增加将CSV文件加载到DataTable中
 * 2、增加将DataTable中的数据保存到CSV文件中
 * 版本：V1.0.0
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soso.Common.FileHelp
{
    /// <summary>
    /// CSV文件帮助类
    /// </summary>
    /// <remarks>
    /// 能对CSV文件进行保存数据的操作及将CSV转换成DataTable的操作
    /// </remarks>
    /// <seealso cref="File"/>
    /// <seealso cref="DataTable"/>
    public sealed class CsvHelp
    {
        /// <summary>
        /// 保存数据到CSV文件中
        /// </summary>
        /// <param name="path">文件保存的路径</param>
        /// <param name="fileName">文件保存名称，不用带后缀</param>
        /// <param name="content">保存的数据：用英文逗号分隔</param>
        /// <param name="title">每列数据的标题，如果第一次保存必须输入</param>
        /// <param name="errorInfo">错误信息</param>
        /// <returns>是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        /// <seealso cref="File.AppendAllLines(string, IEnumerable{string}, Encoding)"/>
        public static bool SaveToCsvFile(string path, string fileName, string content, out string errorInfo, string title = "")
        {
            errorInfo = string.Empty;

            try
            {
                string fileFullName = Path.Combine(path, fileName + ".csv");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!File.Exists(fileFullName))
                {
                    if (title == "")
                    {
                        errorInfo = "The first save must provide a title for each column of data!!";
                        return false;
                    }
                    else
                    {
                        File.AppendAllLines(fileFullName, new string[] { title }, Encoding.UTF8);
                    }
                }

                File.AppendAllLines(fileFullName, new string[] { content }, Encoding.UTF8);

            }
            catch (Exception ex)
            {
                errorInfo = "An anomaly has occurred:" + ex.Message;
                return false;
            }
            errorInfo = "Data saved successfully!";
            return true;
        }

        /// <summary>
        /// 加载CSV文件，并将数据存储到一个<see cref="DataTable"/>中
        /// </summary>
        /// <remarks>
        /// 通过加载一个合法的CSV文件，读取文件中的数据，将表头保存到<see cref="DataTable"/>的<see cref="DataTable.Columns"/>中<br/>
        /// 将所有的数据行保存到<see cref="DataTable"/>中的<see cref="DataTable.Rows"/>中，便于绑定数据到相关的UI表格中
        /// </remarks>
        /// <param name="fileName">文件名</param>
        /// <returns>返回一个<see cref="DataTable"/>，如果CSV文件中数据异常反回<see langword="null"/></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="FileLoadException"/>
        public static async Task<DataTable?> LoadCSVToDataTableAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("The file name cannot be empty or null!");
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"The file {fileName} not found!");
            }

            if (Path.GetExtension(fileName).ToLower() != ".csv")
            {
                throw new FileLoadException($"The file {fileName} isn't csv file!");
            }

            DataTable dt = new DataTable();
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader read = new StreamReader(stream, Encoding.Default))
                    {
                        string? headers = await read.ReadLineAsync();
                        AddHeaderToDataTable(dt, headers);

                        string? line = null;
                        while ((line = await read.ReadLineAsync()) != null)
                        {
                            AddRowDataToDataTable(dt, line);
                        }
                    }
                }

                return dt;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        /// <summary>
        /// 将<see cref="DataTable"/>中的数据保存到CSV文件中
        /// </summary>
        /// <remarks>
        /// 将<see cref="DataTable"/>中的数据保存到CSV文件中，<see cref="DataTable"/>中的<see cref="DataTable.Columns"/>作为文件的表头<br/>
        /// <see cref="DataTable"/>中的<see cref="DataTable.Rows"/>作为文件的内容
        /// </remarks>
        /// <param name="dt">数据源</param>
        /// <param name="path">保存路径</param>
        /// <param name="fileName">保存文件名</param>
        /// <param name="isOverlay">是否覆盖存在文件</param>
        /// <param name="errorInfo">错误信息</param>
        /// <returns>是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public static bool SaveDataTableToCSV(DataTable dt, string path, string fileName, out string errorInfo, bool isOverlay = false)
        {
            errorInfo = string.Empty;

            try
            {
                string fileFullName = Path.Combine(path, fileName + ".csv");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(fileFullName))
                {
                    if (isOverlay)
                    {
                        File.Delete(fileFullName);
                    }
                    else
                    {
                        errorInfo = "File exists, please confirm is overlay!";
                        return false;
                    }
                }

                if (dt.Columns.Count < 0)
                {
                    errorInfo = "DataTable's data must contain columns headers!";
                    return false;
                }

                // 将DataTable中的Columns表头转为string数组
                var columns = dt.Columns.Cast<object>().ToList().Select(item => item.ToString());

                File.AppendAllLines(fileFullName, new string[] { string.Join(",", columns) }, Encoding.UTF8);

                foreach (var item in dt.Rows)
                {
                    // 将DataTable中的Rows数据转为string数组
                    var dataRow = item as DataRow;
                    var rows = dataRow?.ItemArray.Select(item => item?.ToString());

                    if (rows != null)
                    {
                        File.AppendAllLines(fileFullName, new string[] { string.Join(",", rows) }, Encoding.UTF8);
                    }
                }
            }
            catch (Exception ex)
            {
                errorInfo = "An anomaly has occurred:" + ex.Message;
                return false;
            }

            errorInfo = "Data saved successfully!";
            return true;
        }

        private static void AddHeaderToDataTable(DataTable dt, string? headerStr)
        {
            if (string.IsNullOrEmpty(headerStr))
            {
                throw new ArgumentNullException("The datatable's headers cannot be empty or null!");
            }

            var headers = headerStr.Split(',');
            foreach (var header in headers)
            {
                dt.Columns.Add(header);
            }

        }

        private static void AddRowDataToDataTable(DataTable dt, string rowStr)
        {
            DataRow dataRow = dt.NewRow();
            var values = rowStr.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                dataRow[i] = values[i].Trim();
            }
            dt.Rows.Add(dataRow);
        }
    }
}