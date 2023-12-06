#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test
 * 唯一标识：c3cba252-d732-4c7b-b6c5-fdf52575bcd6
 * 文件名：SosoObjectTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/10/2023 10:07:00 AM
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

using System.Data;

namespace Soso.Common.Test
{
    [TestClass]
    public class SosoObjectTest
    {
        [TestMethod]
        public void NonParamCtorTest()
        {
            var obj = new SosoObject();
            Assert.AreEqual(0, obj.I);
            Assert.AreEqual(0, obj.D);
            Assert.AreEqual(false, obj.B);
            Assert.AreEqual(string.Empty, obj.S);
            Assert.IsFalse(obj.IsNumber);
            Assert.IsTrue(obj.IsNullOrEmpty);
            Assert.AreEqual(string.Empty, obj.ToString());
            Assert.AreEqual(string.Empty, obj.Clone().ToString());
            Assert.AreEqual(-1, obj.CompareTo(3));
            Assert.AreEqual(0, obj.CompareTo(string.Empty));
            Assert.ThrowsException<ArgumentNullException>(() => obj.CompareTo(null));
            Assert.AreEqual(0, obj.CompareTo(new SosoObject()));
            Assert.AreEqual(-1, obj.CompareTo(new SosoObject("a")));
            Assert.AreEqual(true, obj.Equals(new SosoObject()));
            Assert.AreEqual(false, obj.Equals(new SosoObject(3)));
            Assert.AreEqual(string.Empty.GetHashCode(), obj.GetHashCode());
        }

        [TestMethod]
        public void IntParamCtorTest()
        {
            var obj = new SosoObject(15);
            Assert.AreEqual(15, obj.I);
            Assert.AreEqual(15, obj.D);
            Assert.AreEqual(false, obj.B);
            Assert.AreEqual("15", obj.S);
            Assert.IsTrue(obj.IsNumber);
            Assert.IsFalse(obj.IsNullOrEmpty);
            Assert.AreEqual("15", obj.Clone().ToString());
            Assert.AreEqual(-1, obj.CompareTo(16));
            Assert.AreEqual(0, obj.CompareTo(15));
            Assert.AreEqual(1, obj.CompareTo(14));
            Assert.AreEqual(-1, obj.CompareTo("A"));
            Assert.AreEqual(1, obj.CompareTo("+"));
            Assert.AreEqual(-1, obj.CompareTo(new SosoObject(16)));
            Assert.AreEqual(0, obj.CompareTo(new SosoObject(15)));
            Assert.AreEqual(1, obj.CompareTo(new SosoObject(14)));
            Assert.IsTrue(obj.Equals(15));
            Assert.IsFalse(obj.Equals(16));
            Assert.IsTrue(obj.Equals(new SosoObject(15)));
            Assert.IsFalse(obj.Equals(new SosoObject(16)));
            Assert.AreEqual(new SosoObject(15), new SosoObject("15"));
            Assert.AreEqual("15".GetHashCode(), obj.GetHashCode());
        }

        [TestMethod]
        public void DoubleParamCtorTest()
        {
            var obj = new SosoObject(15.5);
            Assert.AreEqual(0, obj.I);
            Assert.AreEqual(15.5, obj.D);
            Assert.AreEqual(false, obj.B);
            Assert.AreEqual("15.5", obj.S);
            Assert.IsTrue(obj.IsNumber);
            Assert.IsFalse(obj.IsNullOrEmpty);
            Assert.AreEqual("15.5", obj.Clone().ToString());
            Assert.AreEqual(-1, obj.CompareTo(16));
            Assert.AreEqual(0, obj.CompareTo(15.5));
            Assert.AreEqual(1, obj.CompareTo(14));
            Assert.AreEqual(-1, obj.CompareTo("A"));
            Assert.AreEqual(1, obj.CompareTo("+"));
            Assert.AreEqual(-1, obj.CompareTo(new SosoObject(16)));
            Assert.AreEqual(0, obj.CompareTo(new SosoObject(15.5)));
            Assert.AreEqual(1, obj.CompareTo(new SosoObject(14)));
            Assert.IsTrue(obj.Equals(15.5));
            Assert.IsFalse(obj.Equals(16));
            Assert.IsTrue(obj.Equals(new SosoObject(15.5)));
            Assert.IsFalse(obj.Equals(new SosoObject(16)));
            Assert.AreEqual(new SosoObject(15.5), new SosoObject("15.5"));
            Assert.AreEqual("15.5".GetHashCode(), obj.GetHashCode());
        }

        [TestMethod]
        public void BoolParamCtorTest()
        {
            var obj = new SosoObject(true);
            Assert.AreEqual(0, obj.I);
            Assert.AreEqual(0, obj.D);
            Assert.AreEqual(true, obj.B);
            Assert.AreEqual("True", obj.S);
            Assert.IsFalse(obj.IsNumber);
            Assert.IsFalse(obj.IsNullOrEmpty);
            Assert.AreEqual("True", obj.Clone().ToString());
            Assert.AreEqual(-1, obj.CompareTo("U"));
            Assert.AreEqual(0, obj.CompareTo(true));
            Assert.AreEqual(1, obj.CompareTo("S"));
            Assert.AreEqual(-1, obj.CompareTo("U"));
            Assert.AreEqual(1, obj.CompareTo("S"));
            Assert.AreEqual(-1, obj.CompareTo(new SosoObject("U")));
            Assert.AreEqual(0, obj.CompareTo(new SosoObject(true)));
            Assert.AreEqual(1, obj.CompareTo(new SosoObject("S")));
            Assert.IsTrue(obj.Equals(true));
            Assert.IsFalse(obj.Equals(false));
            Assert.IsTrue(obj.Equals(new SosoObject(true)));
            Assert.IsFalse(obj.Equals(new SosoObject(16)));
            Assert.AreEqual(new SosoObject(true), new SosoObject("True"));
            Assert.AreEqual("True".GetHashCode(), obj.GetHashCode());
        }

        [TestMethod]
        public void StringParamCtorTest()
        {
            var obj = new SosoObject("Test");
            Assert.AreEqual(0, obj.I);
            Assert.AreEqual(0, obj.D);
            Assert.AreEqual(false, obj.B);
            Assert.AreEqual("Test", obj.S);
            Assert.IsFalse(obj.IsNumber);
            Assert.IsFalse(obj.IsNullOrEmpty);
            Assert.AreEqual("Test", obj.Clone().ToString());
            Assert.AreEqual(-1, obj.CompareTo("U"));
            Assert.AreEqual(0, obj.CompareTo("Test"));
            Assert.AreEqual(1, obj.CompareTo("S"));
            Assert.AreEqual(-1, obj.CompareTo("U"));
            Assert.AreEqual(1, obj.CompareTo("S"));
            Assert.AreEqual(-1, obj.CompareTo(new SosoObject("U")));
            Assert.AreEqual(0, obj.CompareTo(new SosoObject("Test")));
            Assert.AreEqual(1, obj.CompareTo(new SosoObject("S")));
            Assert.IsTrue(obj.Equals("Test"));
            Assert.IsFalse(obj.Equals("ABC"));
            Assert.IsTrue(obj.Equals(new SosoObject("Test")));
            Assert.IsFalse(obj.Equals(new SosoObject(16)));
            Assert.AreEqual(new SosoObject("Test"), new SosoObject("Test"));
            Assert.AreEqual("Test".GetHashCode(), obj.GetHashCode());
        }

        [TestMethod]
        public void ImplicitMethodTest()
        {
            SosoObject obj = 5;
            Assert.AreEqual(obj, new SosoObject(5));
            int i = obj;
            Assert.AreEqual(5, i);

            obj = 5.5;
            Assert.AreEqual(obj, new SosoObject(5.5));
            double d = obj;
            Assert.AreEqual(5.5, d);

            obj = false;
            Assert.AreEqual(obj, new SosoObject(false));
            bool b = obj;
            Assert.AreEqual(false, b);

            obj = "Test";
            Assert.AreEqual(obj, new SosoObject("Test"));
            string s = obj;
            Assert.AreEqual("Test", s);
        }

        [TestMethod]
        public void OperatorAddTest()
        {
            Assert.AreEqual(new SosoObject(20), new SosoObject(10) + new SosoObject(10));
            Assert.AreEqual(new SosoObject(20.5), new SosoObject(10) + new SosoObject(10.5));
            Assert.AreEqual(new SosoObject("10True"), new SosoObject(10) + new SosoObject(true));
            Assert.AreEqual(new SosoObject("10Test"), new SosoObject(10) + new SosoObject("Test"));
            Assert.AreEqual(new SosoObject("10.5True"), new SosoObject(10.5) + new SosoObject(true));
            Assert.AreEqual(new SosoObject("10.5Test"), new SosoObject(10.5) + new SosoObject("Test"));
            Assert.AreEqual(new SosoObject("TrueTest"), new SosoObject(true) + new SosoObject("Test"));

            SosoObject obj = 10;
            Assert.AreEqual(new SosoObject(20), obj + 10);
            Assert.AreEqual(new SosoObject(20), 10 + obj);
            Assert.AreEqual(new SosoObject(20.5), obj + 10.5);
            Assert.AreEqual(new SosoObject(20.5), 10.5 + obj);
            Assert.AreEqual(new SosoObject("10True"), obj + true);
            Assert.AreEqual(new SosoObject("False10"), false + obj);
            Assert.AreEqual(new SosoObject("10Test"), obj + "Test");
            Assert.AreEqual(new SosoObject("Test10"), "Test" + obj);
        }

        [TestMethod]
        public void OperatorSubTest()
        {
            SosoObject obj1 = 5;
            SosoObject obj2 = 2;
            Assert.AreEqual(new SosoObject(3), obj1 - obj2);
            Assert.AreEqual(new SosoObject(-3), obj2 - obj1);

            obj1 = 3.5;
            obj2 = 6.7;
            Assert.AreEqual(new SosoObject(-3.2), obj1 - obj2);
            Assert.AreEqual(new SosoObject(3.2), obj2 - obj1);

            obj1 = 4;
            Assert.AreEqual(new SosoObject(-2.7), obj1 - obj2);
            Assert.AreEqual(new SosoObject(2.7), obj2 - obj1);

            obj1 = true;
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 - obj2);
            Assert.ThrowsException<InvalidExpressionException>(() => obj2 - obj1);

            obj1 = "Test";
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 - obj2);
            Assert.ThrowsException<InvalidExpressionException>(() => obj2 - obj1);
        }

        [TestMethod]
        public void OperatorMulTest()
        {
            SosoObject obj1 = 5;
            SosoObject obj2 = 2;
            Assert.AreEqual(new SosoObject(10), obj1 * obj2);

            obj1 = 3.5;
            obj2 = 6.7;
            Assert.AreEqual(new SosoObject(3.5 * 6.7), obj1 * obj2);

            obj1 = 4;
            Assert.AreEqual(new SosoObject(4 * 6.7), obj1 * obj2);

            obj1 = true;
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 * obj2);

            obj1 = "Test";
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 * obj2);
        }

        [TestMethod]
        public void OperatorDivTest()
        {
            SosoObject obj1 = 5;
            SosoObject obj2 = 2;
            Assert.AreEqual(new SosoObject(2.5), obj1 / obj2);

            obj1 = 3.5;
            obj2 = 6.7;
            Assert.AreEqual(new SosoObject(3.5 / 6.7), obj1 / obj2);

            obj1 = 4;
            Assert.AreEqual(new SosoObject(4 / 6.7), obj1 / obj2);

            obj2 = 0;
            Assert.ThrowsException<DivideByZeroException>(() => obj1 / obj2);

            obj1 = true;
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 / obj2);

            obj1 = "Test";
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 / obj2);
        }

        [TestMethod]
        public void CompareOperatorTest()
        {
            SosoObject obj1 = 5;
            SosoObject obj2 = 2;
            Assert.IsTrue(obj1 > obj2);
            Assert.IsTrue(obj2 < obj1);
            Assert.IsTrue(obj1 >= obj2);
            Assert.IsTrue(obj2 <= obj1);
            Assert.IsFalse(obj1 == obj2);
            Assert.IsTrue(obj1 != obj2);
            obj2 = "Test";
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 > obj2);
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 < obj2);
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 >= obj2);
            Assert.ThrowsException<InvalidExpressionException>(() => obj1 <= obj2);
        }

    }
}
