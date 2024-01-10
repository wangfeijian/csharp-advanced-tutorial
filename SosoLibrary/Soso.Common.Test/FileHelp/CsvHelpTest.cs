#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test
 * 唯一标识：09d72948-a139-41b4-a816-14934f5633d1
 * 文件名：CsvHelpTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/27/2023 1:30:06 PM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using Soso.Common.FileHelp;

namespace Soso.Common.Test.FileHelp
{
    [TestClass]
    public class CsvHelpTest
    {
        private const string _path = @"D:\Test";
        private const string _fileName = "test";

        [TestMethod]
        public void SaveToCsvFileTest()
        {
            string error;
            bool result = CsvHelp.SaveToCsvFile(_path, _fileName, "1,2,3,4", out error);
            Assert.IsFalse(result);
            Assert.AreEqual("The first save must provide a title for each column of data!!", error);

            result = CsvHelp.SaveToCsvFile(_path, _fileName, "1,2,3,4", out error, "First,Second,Third,Forth");
            Assert.IsTrue(result);
            Assert.AreEqual("Data saved successfully!", error);

            result = CsvHelp.SaveToCsvFile(_path, _fileName, "5,6,7,8", out error);
            Assert.IsTrue(result);
            Assert.AreEqual("Data saved successfully!", error);
        }

        [TestMethod]
        public void SaveDataTableToCSVTest()
        {
            _ = CsvHelp.SaveToCsvFile(_path, _fileName, "1,2,3,4", out string error, "First,Second,Third,Forth");
            _ = CsvHelp.SaveToCsvFile(_path, _fileName, "5,6,7,8", out error);

            var result = CsvHelp.LoadCsvToDataTableAsync(Path.Combine(_path, _fileName) + ".csv").Result;

            Assert.IsFalse(CsvHelp.SaveDataTableToCsv(result, _path, _fileName, out error));
            Assert.AreEqual("File exists, please confirm is overlay!", error);

            Assert.IsTrue(CsvHelp.SaveDataTableToCsv(result, _path, _fileName, out error, true));
            Assert.AreEqual("Data saved successfully!", error);

            var dataResult = CsvHelp.LoadCsvToDataTableAsync(Path.Combine(_path, _fileName) + ".csv").Result;
            string[] headers = { "First", "Second", "Third", "Forth" };

            Assert.AreEqual(4, dataResult.Columns.Count);
            for (int i = 0; i < headers.Length; i++)
            {
                Assert.AreEqual(headers[i], dataResult.Columns[i].ColumnName);
            }

            string[,] rows = { { "1", "2", "3", "4" }, { "5", "6", "7", "8" } };
            for (int i = 0; i < rows.Rank; i++)
            {
                for (int j = 0; j < rows.GetLength(i); j++)
                {
                    Assert.AreEqual(rows[i, j], dataResult.Rows[i][j]);
                }
            }
        }

        [TestMethod]
        public void LoadCSVToDataTableAsyncTest()
        {
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => CsvHelp.LoadCsvToDataTableAsync(""));
            Assert.ThrowsExceptionAsync<FileNotFoundException>(() => CsvHelp.LoadCsvToDataTableAsync(Path.Combine(_path, _fileName)));
            Assert.ThrowsExceptionAsync<FileLoadException>(() => CsvHelp.LoadCsvToDataTableAsync(@"D:\stpd.dll"));

            _ = CsvHelp.SaveToCsvFile(_path, _fileName, "1,2,3,4", out string error, "First,Second,Third,Forth");
            _ = CsvHelp.SaveToCsvFile(_path, _fileName, "5,6,7,8", out error);

            var result = CsvHelp.LoadCsvToDataTableAsync(Path.Combine(_path, _fileName) + ".csv").Result;
            string[] headers = { "First", "Second", "Third", "Forth" };

            Assert.AreEqual(4, result.Columns.Count);
            for (int i = 0; i < headers.Length; i++)
            {
                Assert.AreEqual(headers[i], result.Columns[i].ColumnName);
            }

            string[,] rows = { { "1", "2", "3", "4" }, { "5", "6", "7", "8" } };
            for (int i = 0; i < rows.Rank; i++)
            {
                for (int j = 0; j < rows.GetLength(i); j++)
                {
                    Assert.AreEqual(rows[i, j], result.Rows[i][j]);
                }
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Directory.Delete(_path, true);
        }
    }
}