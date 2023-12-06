#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test.SystemHelp
 * 唯一标识：f89012cd-8975-4925-aa21-7ece2c4ea565
 * 文件名：SerializationHelpTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/6/2023 3:32:35 PM
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
using Soso.Common.Test.FileHelp;

namespace Soso.Common.Test.SystemHelp
{
    public class TestDeepCopy
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    [TestClass]
    public class SerializationHelpTest
    {
        private const string _fileName = @"D:\test";

        [TestMethod]
        public void DataContractSerializeTest()
        {
            var person = new Person("wangfeijian", 100);
            SerializationHelp.DataContractSerialize(person, _fileName);
            Assert.IsTrue(File.Exists(_fileName));

            var deserializePerson = SerializationHelp.DataContractDeserialize<Person>(_fileName);
            Assert.IsNotNull(deserializePerson);
            Assert.AreEqual(person, deserializePerson);
        }

        [TestMethod]
        public void DeepCopyByXmlSerializerTest()
        {
            var testInstance = new TestDeepCopy() { Age = 100, Name = "wangfeijian" };
            var copyInstance = SerializationHelp.DeepCopyByXmlSerializer(testInstance);
            Assert.IsNotNull(copyInstance);
            Assert.AreEqual(testInstance.Name, copyInstance.Name);
            Assert.AreEqual(testInstance.Age, copyInstance.Age);
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