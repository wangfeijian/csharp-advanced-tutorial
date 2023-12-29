#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test.SystemHelp
 * 唯一标识：9916f260-c3fb-409d-aa28-8646ab7c48ed
 * 文件名：CommonHelpTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/27/2023 10:22:44 AM
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

using Soso.Common.SystemHelp;
using System.Text;

namespace Soso.Common.Test.SystemHelp
{
    [TestClass]
    public class CommonHelpTest
    {
        [TestMethod]
        public void AcsiiStringByteTest()
        {
            string str = "hello";
            var bytes = Encoding.Default.GetBytes(str);
            string result = CommonHelp.ByteToASCIIString(bytes);
            Assert.AreEqual(result.ToUpper(), "68656C6C6F");
            result = CommonHelp.ByteToASCIIString(bytes, 0, bytes.Length);
            Assert.AreEqual(result.ToUpper(), "68 65 6C 6C 6F");

            string asciiStr = "68656C6C6F";
            var byteResult = CommonHelp.ASCIIStringToByte(asciiStr);
            Assert.AreEqual(byteResult.Length, 5);
            asciiStr = "68 65 6C 6C 6F";
            byteResult = CommonHelp.ASCIIStringToByte(asciiStr, " ");
        }

        [TestMethod]
        public void HardSpaceTest()
        {
            var size = CommonHelp.GetHardDiskTotalSpace("C");
            string letter = "B";
            double result = size * 1.0;
            CommonHelp.ByteConvert(ref result, ref letter);
            Assert.AreEqual(letter, "GB");
            Assert.AreEqual((int)result, 249);

            size = CommonHelp.GetHardDiskFreeSpace("C");
            letter = "B";
            result = size * 1.0;
            CommonHelp.ByteConvert(ref result, ref letter);
            Assert.AreEqual(letter, "GB");
            Assert.AreEqual((int)result, 65);
        }

        [TestMethod]
        public void MemoryUsedTest()
        {
            var free = CommonHelp.GetAvailPhys();
            var used = CommonHelp.GetUsedPhys();
            var total = CommonHelp.GetTotalPhys();
            double freeMemory = free * 1.0;
            double usedMemory = used * 1.0;
            double totalMemory = total * 1.0;
            string letter = "B";
            CommonHelp.ByteConvert(ref freeMemory, ref letter);
            letter = "B";
            CommonHelp.ByteConvert(ref usedMemory, ref letter);
            letter = "B";
            CommonHelp.ByteConvert(ref totalMemory, ref letter);

            Assert.AreEqual((int)(freeMemory + usedMemory), (int)totalMemory);

            //Assert.AreEqual(CommonHelp.GetMemortRate(), 0.91);
            var cpu = CommonHelp.GetCpuUsedPercent();
            Assert.AreNotEqual(cpu, 0);
            double ramUsed = 0;
            var ram = CommonHelp.GetRamUsed(ref ramUsed);
            Assert.AreNotEqual(ramUsed, 0);
        }
    }
}
