#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test
 * 唯一标识：08edba87-63af-41e7-8257-fc190ad4037e
 * 文件名：IniHelpTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/27/2023 3:36:02 PM
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

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
namespace Soso.Common.Test.FileHelp
{
    [Serializable]
    public record Person(string Name, int age);


    [TestClass]
    public class IniHelpTest
    {
        private const string _fileName = @"D:\test.ini";
        private IniHelp iniHelp = new IniHelp(_fileName);

        [TestMethod]
        public void InstanceReadWriteStringTest()
        {
            Assert.IsTrue(iniHelp.WriteString("Test", "Color", "Yellow"));

            Assert.AreEqual("", iniHelp.ReadString(null, null, null));
            Assert.AreEqual("", iniHelp.ReadString("Test", null, null));
            Assert.AreEqual("", iniHelp.ReadString(null, "Color", null));
            Assert.AreEqual("Color", iniHelp.ReadString(null, null, "Color"));

            Assert.IsTrue(iniHelp.WriteString("Test", "Color", "红色"));
            Assert.AreEqual("红色", iniHelp.ReadString("Test", "Color", "Color"));

            Assert.IsTrue(iniHelp.WriteString("Test", "Color", "red"));
            Assert.AreEqual("red", iniHelp.ReadString("Test", "Color", "Color"));
        }

        [TestMethod]
        public void InstanceAppendStringTest()
        {
            Assert.IsTrue(iniHelp.WriteString("Test", "Color", "Yellow"));
            Assert.IsTrue(iniHelp.AppendString("Test", "Color", "Color"));
            Assert.AreEqual("YellowColor", iniHelp.ReadString("Test", "Color"));
        }

        [TestMethod]
        public void InstanceReadWriteJsonSerializeTest()
        {
            Assert.IsTrue(iniHelp.WriteObject("Object", "FirstString", new string[] { "first", "second", "third" }));
            Assert.IsTrue(iniHelp.WriteObject("Object", "SecondString", new string[] { }));
            Assert.IsTrue(iniHelp.WriteObject("Object", "FirstInt", new int[] { 1, 2, 3 }));
            Assert.IsTrue(iniHelp.WriteObject("Object", "SecondInt", new int[] { }));
            Assert.IsTrue(iniHelp.WriteObject("Object", "FirstDouble", new double[] { 1.1, 2.2, 3.3 }));
            Assert.IsTrue(iniHelp.WriteObject("Object", "SecondDouble", new double[] { }));
            Assert.IsTrue(iniHelp.WriteObject("Object", "Int", 35));
            Assert.IsTrue(iniHelp.WriteObject("Object", "Double", 99.9));
            Assert.IsTrue(iniHelp.WriteObject("Object", "Person", new Person("wangfeijian", 100)));
            Assert.IsTrue(iniHelp.WriteObject("Object", "PersonArray", new Person[] { new Person("wangfeijian", 100), new Person("wang", 300) }));
            Assert.IsTrue(iniHelp.WriteObject("Object", "string", "Test Json"));
            Assert.IsTrue(iniHelp.WriteObject("Object", "bool", true));

            var firstString = iniHelp.ReadObject<string[]>("Object", "FirstString");
            CheckArrayValueEqauls(new string[] { "first", "second", "third" }, firstString!);

            var secondString = iniHelp.ReadObject<string[]>("Object", "SecondString");
            CheckArrayValueEqauls(new string[] { }, secondString!);

            var firstInt = iniHelp.ReadObject<int[]>("Object", "FirstInt");
            CheckArrayValueEqauls(new int[] { 1, 2, 3 }, firstInt!);

            var secondInt = iniHelp.ReadObject<int[]>("Object", "SecondInt");
            CheckArrayValueEqauls(new int[] { }, secondInt!);

            var firstDouble = iniHelp.ReadObject<double[]>("Object", "FirstDouble");
            CheckArrayValueEqauls(new double[] { 1.1, 2.2, 3.3 }, firstDouble!);

            var secondDouble = iniHelp.ReadObject<double[]>("Object", "SecondDouble");
            CheckArrayValueEqauls(new double[] { }, secondDouble!);

            Assert.AreEqual(35, iniHelp.ReadObject<int>("Object", "Int"));
            Assert.AreEqual(99.9, iniHelp.ReadObject<double>("Object", "Double"));
            Assert.AreEqual(new Person("wangfeijian", 100), iniHelp.ReadObject<Person>("Object", "Person"));
            CheckArrayValueEqauls(new Person[] { new Person("wangfeijian", 100), new Person("wang", 300) }, iniHelp.ReadObject<Person[]>("Object", "PersonArray")!);
            Assert.AreEqual("Test Json", iniHelp.ReadObject<string>("Object", "String"));
            Assert.AreEqual(true, iniHelp.ReadObject<bool>("Object", "bool"));
        }

        [TestMethod]
        public void InstanceGetAllSectionNamesTest()
        {
            WriteSections();
            CheckArrayValueEqauls(new string[] { "Array", "Base" }, iniHelp.GetAllSectionName());
            Assert.IsTrue(iniHelp.IsSectionExist("Array"));
            Assert.IsTrue(iniHelp.IsSectionExist("Base"));
            Assert.IsFalse(iniHelp.IsSectionExist("Test"));
        }

        [TestMethod]
        public void InstanceGetSectionKeysTest()
        {
            WriteSections();
            CheckArrayValueEqauls(new string[] { "FirstString", "SecondString", "FirstInt", "SecondInt", "FirstDouble", "SecondDouble", "PersonArray" }, iniHelp.GetKeysForSection("Array"));
            CheckArrayValueEqauls(new string[] { "Int", "Double", "Person", "string", "bool" }, iniHelp.GetKeysForSection("Base"));
            Assert.IsTrue(iniHelp.IsKeyExist("Array", "PersonArray"));
            Assert.IsTrue(iniHelp.IsKeyExist("Array", "SecondDouble"));
            Assert.IsTrue(iniHelp.IsKeyExist("Array", "FirstDouble"));
            Assert.IsTrue(iniHelp.IsKeyExist("Array", "SecondInt"));
            Assert.IsTrue(iniHelp.IsKeyExist("Array", "FirstInt"));
            Assert.IsTrue(iniHelp.IsKeyExist("Array", "SecondString"));
            Assert.IsTrue(iniHelp.IsKeyExist("Array", "FirstString"));
            Assert.IsFalse(iniHelp.IsKeyExist("Array", "NotInSection"));
            Assert.IsFalse(iniHelp.IsKeyExist("Array", "ErrorSection"));

            Assert.IsTrue(iniHelp.IsKeyExist("Base", "Int"));
            Assert.IsTrue(iniHelp.IsKeyExist("Base", "Double"));
            Assert.IsTrue(iniHelp.IsKeyExist("Base", "Person"));
            Assert.IsTrue(iniHelp.IsKeyExist("Base", "string"));
            Assert.IsTrue(iniHelp.IsKeyExist("Base", "bool"));
            Assert.IsFalse(iniHelp.IsKeyExist("Base", "Error"));
            Assert.IsFalse(iniHelp.IsKeyExist("Base", "Warning"));
        }

        [TestMethod]
        public void InstanceDeleteKeysTest()
        {
            WriteSections();
            Assert.IsTrue(iniHelp.IsKeyExist("Array", "PersonArray"));
            Assert.IsTrue(iniHelp.DeleteKey("Array", "PersonArray"));
            Assert.IsFalse(iniHelp.IsKeyExist("Array", "PersonArray"));

            Assert.IsTrue(iniHelp.IsKeyExist("Base", "Int"));
            Assert.IsTrue(iniHelp.DeleteKey("Base", "Int"));
            Assert.IsFalse(iniHelp.IsKeyExist("Base", "Int"));

            Assert.IsTrue(iniHelp.DeleteAllKeys("Array"));
            Assert.IsTrue(iniHelp.DeleteAllKeys("Base"));

            Assert.IsFalse(iniHelp.IsKeyExist("Base", "Double"));
            Assert.IsFalse(iniHelp.IsKeyExist("Base", "Person"));
            Assert.IsFalse(iniHelp.IsKeyExist("Base", "string"));
            Assert.IsFalse(iniHelp.IsKeyExist("Base", "bool"));

            Assert.IsFalse(iniHelp.IsKeyExist("Array", "SecondDouble"));
            Assert.IsFalse(iniHelp.IsKeyExist("Array", "FirstDouble"));
            Assert.IsFalse(iniHelp.IsKeyExist("Array", "SecondInt"));
            Assert.IsFalse(iniHelp.IsKeyExist("Array", "FirstInt"));
            Assert.IsFalse(iniHelp.IsKeyExist("Array", "SecondString"));
            Assert.IsFalse(iniHelp.IsKeyExist("Array", "FirstString"));
        }

        [TestMethod]
        public void StaticGetAllSectionNamesTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.SetItems(null, "数组", "First=第一个", "Second=第二个", "Third=第三个"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.SetItems(_fileName, null, "First=第一个", "Second=第二个", "Third=第三个"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.SetItems(_fileName, "数组", null));

            Assert.IsTrue(IniHelp.SetItems(_fileName, "数组", "First=第一个", "Second=第二个", "Third=第三个"));
            Assert.IsTrue(IniHelp.SetItems(_fileName, "基础", "First=第一个", "Second=第二个", "Third=第三个"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetAllSectionName(null));
            CheckArrayValueEqauls(new string[] { "数组", "基础" }, IniHelp.GetAllSectionName(_fileName));
        }

        [TestMethod]
        public void StaticGetAllItemsTest()
        {
            Assert.IsTrue(IniHelp.SetItems(_fileName, "Array", "First=第一个", "Second=第二个", "Third=第三个"));
            Assert.IsTrue(IniHelp.SetItems(_fileName, "Base", "First=第一个", "Second=第二个", "Third=第三个"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetAllItems(null, "Base"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetAllItems(_fileName, null));

            CheckArrayValueEqauls(new string[] { "First=第一个", "Second=第二个", "Third=第三个" }, IniHelp.GetAllItems(_fileName, "Base"));
            CheckArrayValueEqauls(new string[] { "First=第一个", "Second=第二个", "Third=第三个" }, IniHelp.GetAllItems(_fileName, "Array"));
        }

        [TestMethod]
        public void StaticGetSectionsKeysTest()
        {
            WriteItems();

            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetSectionKeys(null, "Base"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetSectionKeys(_fileName, null));
            CheckArrayValueEqauls(new string[] { "First", "Second", "Third" }, IniHelp.GetSectionKeys(_fileName, "Array"));
            CheckArrayValueEqauls(new string[] { "First", "Second", "Third" }, IniHelp.GetSectionKeys(_fileName, "Base"));
        }

        [TestMethod]
        public void StaticGetSetValueTest()
        {
            WriteItems();

            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.SetValue(null, "Array", "First", "first"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.SetValue(_fileName, "Array", "First", null));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.SetValue(_fileName, "Array", null, "first"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.SetValue(_fileName, null, "First", "first"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetValue(null, "Array", "First", "first"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetValue(_fileName, "Array", "First", null));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetValue(_fileName, "Array", null, "first"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.GetValue(_fileName, null, "First", "first"));

            Assert.AreEqual("第一个", IniHelp.GetValue(_fileName, "Array", "First"));
            Assert.AreEqual("第一个", IniHelp.GetValue(_fileName, "Base", "First"));
            Assert.IsTrue(IniHelp.SetValue(_fileName, "Array", "First", "Test Array"));
            Assert.IsTrue(IniHelp.SetValue(_fileName, "Base", "First", "Test Base"));
            Assert.AreEqual("Test Array", IniHelp.GetValue(_fileName, "Array", "First"));
            Assert.AreEqual("Test Base", IniHelp.GetValue(_fileName, "Base", "First"));
        }

        [TestMethod]
        public void StaticRemoveMethodTest()
        {
            WriteItems();

            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.RemoveKey(null, "Array", "First"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.RemoveKey(_fileName, null, "First"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.RemoveKey(_fileName, "Array", null));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.RemoveSection(null, "Array"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.RemoveSection(_fileName, null));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.ClearSection(null, "Array"));
            Assert.ThrowsException<ArgumentNullException>(() => IniHelp.ClearSection(_fileName, null));

            Assert.IsTrue(IniHelp.RemoveKey(_fileName, "Array", "First"));
            Assert.IsTrue(IniHelp.RemoveKey(_fileName, "Base", "First"));
            Assert.IsTrue(IniHelp.ClearSection(_fileName, "Array"));
            Assert.IsTrue(IniHelp.ClearSection(_fileName, "Base"));
            Assert.IsTrue(IniHelp.RemoveSection(_fileName, "Array"));
            Assert.IsTrue(IniHelp.RemoveSection(_fileName, "Base"));
        }

        private void WriteSections()
        {
            iniHelp.WriteObject("Array", "FirstString", new string[] { "first", "second", "third" });
            iniHelp.WriteObject("Array", "SecondString", new string[] { });
            iniHelp.WriteObject("Array", "FirstInt", new int[] { 1, 2, 3 });
            iniHelp.WriteObject("Array", "SecondInt", new int[] { });
            iniHelp.WriteObject("Array", "FirstDouble", new double[] { 1.1, 2.2, 3.3 });
            iniHelp.WriteObject("Array", "SecondDouble", new double[] { });
            iniHelp.WriteObject("Base", "Int", 35);
            iniHelp.WriteObject("Base", "Double", 99.9);
            iniHelp.WriteObject("Base", "Person", new Person("wangfeijian", 100));
            iniHelp.WriteObject("Array", "PersonArray", new Person[] { new Person("wangfeijian", 100), new Person("wang", 300) });
            iniHelp.WriteObject("Base", "string", "Test Json");
            iniHelp.WriteObject("Base", "bool", true);
        }

        private void WriteItems()
        {
            IniHelp.SetItems(_fileName, "Array", "First=第一个", "Second=第二个", "Third=第三个");
            IniHelp.SetItems(_fileName, "Base", "First=第一个", "Second=第二个", "Third=第三个");

        }

        private void CheckArrayValueEqauls<T>(T[] source, T[] target)
        {
            Assert.IsTrue(source.Length == target.Length);

            for (int i = 0; i < source.Length; i++)
            {
                Assert.AreEqual(source[i], target[i]);
            }
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
            else
            {
                File.Delete(_fileName);
            }
        }
    }
}