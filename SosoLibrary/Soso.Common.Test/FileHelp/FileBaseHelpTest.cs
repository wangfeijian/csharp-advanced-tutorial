#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test.FileHelp
 * 唯一标识：161f86a9-f54a-4356-95a2-9f09cb05648b
 * 文件名：FileBaseTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/29/2023 1:25:58 PM
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

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
using Soso.Common.FileHelp;

namespace Soso.Common.Test.FileHelp
{
    [TestClass]
    public class FileBaseHelpTest
    {
        private const string _fileName = @"D:\Test\test.txt";

        [TestMethod]
        public void CreateDirForFullNameTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => FileBaseHelp.CreateDirForFullName(null));
            Assert.ThrowsException<ArgumentNullException>(() => FileBaseHelp.CreateDirForFullName("test"));
            FileBaseHelp.CreateDirForFullName(_fileName);
            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(_fileName)));
        }

        [TestCleanup]
        public void Cleanup()
        {
            var file = new FileInfo(_fileName);
            var result = file.DirectoryName!.Split("\\");
            if (result[1] != "")
            {
                Directory.Delete(file.DirectoryName, true);
            }
        }
    }
}