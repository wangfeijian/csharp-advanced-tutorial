#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test.SystemHelp
 * 唯一标识：c478473e-559a-4247-8f8d-2d05ad5d3fed
 * 文件名：RegisterHelpTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/12/2023 2:28:48 PM
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
    public class RegisterHelpTest
    {
        private string? _boolChangeStr;
        private string? _intChangeStr;
        private string? _doubleChangeStr;
        private string? _stringChangeStr;

        [TestMethod]
        public void SingleIntanceTest()
        {
            Assert.AreEqual(RegisterHelp.Instance, RegisterHelp.Instance);
        }

        [TestMethod]
        public void BoolValueChangeTest()
        {
            RegisterHelp.Instance.BoolRegisterChanged += Instance_BoolRegisterChanged;
            RegisterHelp.Instance.SetBoolRegister(0, false);
            Assert.IsFalse(RegisterHelp.Instance.GetBoolRegister(0));
            RegisterHelp.Instance.SetBoolRegister(1, true);
            Assert.IsTrue(RegisterHelp.Instance.GetBoolRegister(1));
            RegisterHelp.Instance.SetBoolRegister(0, true);
            Assert.IsTrue(RegisterHelp.Instance.GetBoolRegister(0));
            RegisterHelp.Instance.SetBoolRegister(1, false);
            Assert.IsFalse(RegisterHelp.Instance.GetBoolRegister(1));
            Assert.IsFalse(RegisterHelp.Instance.GetBoolRegister(2));
            RegisterHelp.Instance.SetBoolRegister(3, true, true);
            Assert.IsTrue(RegisterHelp.Instance.GetBoolRegister(3));
            Assert.AreEqual(_boolChangeStr, "3_False_True");
            RegisterHelp.Instance.SetBoolRegister(3, true, true);
            Assert.AreEqual(_boolChangeStr, "3_False_True");
            RegisterHelp.Instance.SetBoolRegister(3, false, true);
            Assert.AreEqual(_boolChangeStr, "3_True_False");
        }

        private void Instance_BoolRegisterChanged(int index, bool oldValue, bool newValue)
        {
            _boolChangeStr = $"{index}_{oldValue}_{newValue}";
        }

        [TestMethod]
        public void IntValueChangeTest()
        {
            RegisterHelp.Instance.IntRegisterChanged += Instance_IntRegisterChanged;
            RegisterHelp.Instance.SetIntRegister(0, 12);
            Assert.AreEqual(12, RegisterHelp.Instance.GetIntRegister(0));
            RegisterHelp.Instance.SetIntRegister(1, 22);
            Assert.AreEqual(22, RegisterHelp.Instance.GetIntRegister(1));
            RegisterHelp.Instance.SetIntRegister(0, 15);
            Assert.AreEqual(15, RegisterHelp.Instance.GetIntRegister(0));
            RegisterHelp.Instance.SetIntRegister(1, 25);
            Assert.AreEqual(25, RegisterHelp.Instance.GetIntRegister(1));
            Assert.AreEqual(0, RegisterHelp.Instance.GetIntRegister(2));
            RegisterHelp.Instance.SetIntRegister(3, 20, true);
            Assert.AreEqual(20, RegisterHelp.Instance.GetIntRegister(3));
            Assert.AreEqual(_intChangeStr, "3_0_20");
            RegisterHelp.Instance.SetIntRegister(3, 20, true);
            Assert.AreEqual(_intChangeStr, "3_0_20");
            RegisterHelp.Instance.SetIntRegister(3, 40, true);
            Assert.AreEqual(_intChangeStr, "3_20_40");
        }

        private void Instance_IntRegisterChanged(int index, int oldValue, int newValue)
        {
            _intChangeStr = $"{index}_{oldValue}_{newValue}";
        }

        [TestMethod]
        public void DoubleValueChangeTest()
        {
            RegisterHelp.Instance.DoubleRegisterChanged += Instance_DoubleRegisterChanged;
            RegisterHelp.Instance.SetDoubleRegister(0, 12.5);
            Assert.AreEqual(12.5, RegisterHelp.Instance.GetDoubleRegister(0));
            RegisterHelp.Instance.SetDoubleRegister(1, 22.5);
            Assert.AreEqual(22.5, RegisterHelp.Instance.GetDoubleRegister(1));
            RegisterHelp.Instance.SetDoubleRegister(0, 15.5);
            Assert.AreEqual(15.5, RegisterHelp.Instance.GetDoubleRegister(0));
            RegisterHelp.Instance.SetDoubleRegister(1, 25.5);
            Assert.AreEqual(25.5, RegisterHelp.Instance.GetDoubleRegister(1));
            Assert.AreEqual(0, RegisterHelp.Instance.GetDoubleRegister(2));
            RegisterHelp.Instance.SetDoubleRegister(3, 20.5, true);
            Assert.AreEqual(20.5, RegisterHelp.Instance.GetDoubleRegister(3));
            Assert.AreEqual(_doubleChangeStr, "3_0_20.5");
            RegisterHelp.Instance.SetDoubleRegister(3, 20.5, true);
            Assert.AreEqual(_doubleChangeStr, "3_0_20.5");
            RegisterHelp.Instance.SetDoubleRegister(3, 40.5, true);
            Assert.AreEqual(_doubleChangeStr, "3_20.5_40.5");
        }

        private void Instance_DoubleRegisterChanged(int index, double oldValue, double newValue)
        {
            _doubleChangeStr = $"{index}_{oldValue}_{newValue}";
        }

        [TestMethod]
        public void StringValueChangeTest()
        {
            RegisterHelp.Instance.StringRegisterChanged += Instance_StringRegisterChanged;
            RegisterHelp.Instance.SetStringRegister(0, "wang");
            Assert.AreEqual("wang", RegisterHelp.Instance.GetStringRegister(0));
            RegisterHelp.Instance.SetStringRegister(1, "fei");
            Assert.AreEqual("fei", RegisterHelp.Instance.GetStringRegister(1));
            RegisterHelp.Instance.SetStringRegister(0, "jian");
            Assert.AreEqual("jian", RegisterHelp.Instance.GetStringRegister(0));
            RegisterHelp.Instance.SetStringRegister(1, "wang");
            Assert.AreEqual("wang", RegisterHelp.Instance.GetStringRegister(1));
            Assert.AreEqual("", RegisterHelp.Instance.GetStringRegister(2));
            RegisterHelp.Instance.SetStringRegister(3, "wangfei", true);
            Assert.AreEqual("wangfei", RegisterHelp.Instance.GetStringRegister(3));
            Assert.AreEqual(_stringChangeStr, "3__wangfei");
            RegisterHelp.Instance.SetStringRegister(3, "wangfei", true);
            Assert.AreEqual(_stringChangeStr, "3__wangfei");
            RegisterHelp.Instance.SetStringRegister(3, "wangfeijian", true);
            Assert.AreEqual(_stringChangeStr, "3_wangfei_wangfeijian");
        }

        private void Instance_StringRegisterChanged(int index, string oldValue, string newValue)
        {
            _stringChangeStr = $"{index}_{oldValue}_{newValue}";
        }
    }
}