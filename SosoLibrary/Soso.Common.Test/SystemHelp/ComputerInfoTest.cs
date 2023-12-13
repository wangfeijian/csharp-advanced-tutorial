using Soso.Common.SystemHelp;

namespace Soso.Common.Test.SystemHelp
{
    [TestClass]
    public class ComputerInfoTest
    {
        [TestMethod]
        public void GetSystemMemorySizeTest()
        {
            double value = 16 * 1024 * 1024 * 1024d;
            Assert.AreEqual(value, ComputerInfo.GetSystemMemorySize());
            string unit = "B";
            ComputerInfo.ByteConvert(ref value, ref unit);
            Assert.AreEqual(unit, "GB");
            Assert.AreEqual(16, value);

            value = 2048;
            unit = "B";
            ComputerInfo.ByteConvert(ref value, ref unit);
            Assert.AreEqual(unit, "KB");
            Assert.AreEqual(2, value);

            var result = ComputerInfo.GetDiskSizeAndSerialNumber();
            Assert.AreEqual(result.Count(), 1);
            value = result.First().Value;
            unit = "B";
            ComputerInfo.ByteConvert(ref value, ref unit);
            value = Math.Round(value, 2);
            Assert.AreEqual(value, 931.51);


        }

        [TestMethod]
        public void GetCpuAndDiskInfo()
        {
            Assert.AreEqual("Windows 11 Pro", ComputerInfo.GetOSVersion());
            Assert.AreEqual(ComputerInfo.GetPhysicalNetworkCardMacAddress().Count, 2);

            Assert.AreEqual("WANGFEIJIAN", ComputerInfo.GetComputerName());

            var cpuinfo = ComputerInfo.GetCPUInfo();
            Assert.AreEqual(5, cpuinfo.Count());
            Assert.AreEqual("12288", cpuinfo["L3CacheSize"]);
            Assert.AreEqual("9216", cpuinfo["L2CacheSize"]);
            Assert.AreEqual("16", cpuinfo["ThreadCount"]);
            Assert.AreEqual("BFEBFBFF000906A3", cpuinfo["ProcessorId"]);
            Assert.AreEqual("12th Gen Intel(R) Core(TM) i5-1240P", cpuinfo["Name"]);
        }
    }
}
