#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test.SystemHelp
 * 唯一标识：ea09f532-2587-4b3a-aca0-72e566b4ae29
 * 文件名：MathHelpTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/14/2023 11:36:43 AM
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
    public class MathHelpTest
    {
        [TestMethod]
        public void DegAndRadTest()
        {
            double deg = 28.64789;
            double rad = 0.5;
            for (int i = -13; i < 14; i++)
            {
                Assert.AreEqual(deg * i, Math.Round(MathHelp.RadToDeg(rad * i), 5));
                Assert.AreEqual(rad * i, Math.Round(MathHelp.DegToRad(deg * i), 5));
            }
        }

        [TestMethod]
        public void DegRadNormalizationTest()
        {
            for (int i = -720; i <= 720; i++)
            {
                double temp = MathHelp.DegNormalization(i);
                double left = MathHelp.DegToRad(temp);
                double right = MathHelp.RadNormalization(MathHelp.DegToRad(i));
                Assert.AreEqual(Math.Round(left, 5), Math.Round(right, 5));
                Assert.AreEqual(Math.Round(Math.Sin(left), 5), Math.Round(Math.Sin(right), 5));
                Assert.AreEqual(Math.Round(Math.Cos(left), 5), Math.Round(Math.Cos(right), 5));
            }
        }

        [TestMethod]
        public void FitPlaneTest()
        {
            double[] x = { 10.1, 15.1, 13.1, 10.1, 14.1, 16.1, 32.1 };
            double[] y = { 20.5, 34.5, 7.5, 25.5, 10.5, 40.5, 10.5 };
            double[] z = { 0.12, 0.1, -0.05, 0.03, 0.1, 0.2, -0.2 };
            var plane = MathHelp.FitPlane(x, y, z);
            Assert.AreEqual(4, plane.Length);
            Assert.AreEqual(-0.00983891, Math.Round(plane[0], 8));
            Assert.AreEqual(0.00561816, Math.Round(plane[1], 8));
            Assert.AreEqual(-1, plane[2]);
            Assert.AreEqual(0.0784646, Math.Round(plane[3], 7));

            double[] x1 = { 562.453, 563.242, 562.502, 515.597, 444.404, 432.57, 328.123, 328.121, 325.864 };
            double[] y1 = { 499.594, 547.877, 586.319, 565.321, 566.068, 589.018, 588.617, 551.099, 500.622 };
            double[] z1 = { -824.739, -824.366, -824.24, -824.533, -824.762, -824.748, -824.741, -824.939, -825.234 };
            plane = MathHelp.FitPlane(x1, y1, z1);
            Assert.AreEqual(4, plane.Length);
            Assert.AreEqual(0.002, Math.Round(plane[0], 3));
            Assert.AreEqual(0.004, Math.Round(plane[1], 3));
            Assert.AreEqual(-1, plane[2]);
            Assert.AreEqual(-828.213, Math.Round(plane[3], 3));

            var flatness = MathHelp.Flatness(plane, x1, y1, z1);
            Assert.AreEqual(0.27, Math.Round(flatness, 3));
            double[] result = { 0.042, -0.112, -0.068, 0.025, 0.096, 0.158, -0.088, -0.058, 0.005 };
            for (int i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(result[i], Math.Round(MathHelp.PointToPlane(plane, x1[i], y1[i], z1[i]), 3));
            }

            double[] plane1 = { 1, 2, -1, -2 };
            double[] plane2 = { 2, 1, 1, -19 };
            Assert.AreEqual(60, MathHelp.AngleOfTwoPlane(plane1, plane2));
        }
    }
}