#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test.SystemHelp
 * 唯一标识：0ab35a35-2f09-45c6-848a-26f10ca0d311
 * 文件名：PerformanceTimerTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/13/2023 10:45:28 AM
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

namespace Soso.Common.Test.SystemHelp
{
    [TestClass]
    public class PerformanceTimerTest
    {
        [TestMethod]
        public void IsRunningTest()
        {
            var time = new PerformanceTimer();
            Assert.IsFalse(time.IsRunning);
            time.Start();
            Assert.IsTrue(time.IsRunning);
            time.Stop();
            Assert.IsFalse(time.IsRunning);
            time.Restart();
            Assert.IsTrue(time.IsRunning);
        }

        [TestMethod]
        public void ElapsedTest()
        {
            var time = new PerformanceTimer();
            Assert.AreEqual(new TimeSpan(0, 0, 0), time.Elapsed);
            Assert.AreEqual(0, time.ElapsedSeconds);
            Assert.AreEqual(0, time.ElapsedMilliseconds);
            Assert.AreEqual(0, time.ElapsedMicroseconds);
            Assert.AreEqual(0, time.ElapsedNanoseconds);
            time.Start();
            Assert.IsTrue(new TimeSpan(0, 0, 0) < time.Elapsed);
            Assert.IsTrue(0 < time.ElapsedSeconds);
            Assert.IsTrue(0 < time.ElapsedMilliseconds);
            Assert.IsTrue(0 < time.ElapsedMicroseconds);
            Assert.IsTrue(0 < time.ElapsedNanoseconds);
            var time1 = time.Elapsed;
            var second = time.ElapsedSeconds;
            var millSecond = time.ElapsedMilliseconds;
            var microSecond = time.ElapsedMicroseconds;
            var nanoSecond = time.ElapsedNanoseconds;

            Thread.Sleep(10);
            Assert.IsTrue(time.Elapsed > time1);
            Assert.IsTrue(second < time.ElapsedSeconds);
            Assert.IsTrue(millSecond < time.ElapsedMilliseconds);
            Assert.IsTrue(microSecond < time.ElapsedMicroseconds);
            Assert.IsTrue(nanoSecond < time.ElapsedNanoseconds);
            time.Stop();
            time1 = time.Elapsed;
            second = time.ElapsedSeconds;
            millSecond = time.ElapsedMilliseconds;
            microSecond = time.ElapsedMicroseconds;
            nanoSecond = time.ElapsedNanoseconds;

            Assert.IsTrue(time.Elapsed == time1);
            Assert.IsTrue(second == time.ElapsedSeconds);
            Assert.IsTrue(millSecond == time.ElapsedMilliseconds);
            Assert.IsTrue(microSecond == time.ElapsedMicroseconds);
            Assert.IsTrue(nanoSecond == time.ElapsedNanoseconds);
        }

        [TestMethod]
        public void StaticTest()
        {
            var time = PerformanceTimer.StartNew();
            Assert.IsTrue(time.IsRunning);
            time.Stop();
            Assert.IsFalse(time.IsRunning);

            var timeSpan = PerformanceTimer.Execute(null);

            timeSpan = PerformanceTimer.Execute(() => Thread.Sleep(1000));
            Assert.IsTrue(timeSpan > new TimeSpan(0));
            Assert.IsTrue(timeSpan.Seconds == 1);
            Assert.IsTrue(timeSpan.TotalMilliseconds >= 1000);
        }
    }
}