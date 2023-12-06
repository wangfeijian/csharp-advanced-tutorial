#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.Test.FileHelp
 * 唯一标识：fb453fee-2130-48bc-937a-6d5903b8716a
 * 文件名：XmlHelpTest
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/29/2023 1:04:25 PM
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
    public class XmlHelpTest
    {
        private const string _fileName = @"D:\test.xml";
        private XmlHelp _help;

        [TestMethod]
        public void CtorTest()
        {
            Assert.ThrowsException<FileNotFoundException>(() => new XmlHelp(_fileName));
            Assert.ThrowsException<FileLoadException>(() => new XmlHelp("D:\\stpd.dll"));
            _ = new XmlHelp(_fileName, "root");
            Assert.IsTrue(File.Exists(_fileName));
            Assert.IsNotNull(new XmlHelp(_fileName));
        }

        [TestMethod]
        public void CreateXElementsTest()
        {
            InitXmlFile();

            Assert.ThrowsException<ArgumentNullException>(() => _help.CreateXElement("", null));
            var xElementRoot = _help.GetXElements("root", false).First();
            var xElementChild = _help.CreateXElement("Data", new Dictionary<string, string> { { "First", "one" }, { "Second", "two" } });
            xElementRoot.Add(xElementChild);
            _help.Save();

            Assert.AreEqual(xElementChild.ToString(), new XmlHelp(_fileName).GetXElements("Data", true, "First", "one").First().ToString());
        }

        [TestMethod]
        public void GetXElementsTest()
        {
            InitXmlFile();

            Assert.AreEqual(1, _help.GetXElements("TcpLink", false).Count());
            Assert.AreEqual(1, _help.GetXElements("Data", false).Count());

            Assert.AreEqual(1, _help.GetXElements("TcpLink", true, "名称", "EPSON机器人").Count());
            Assert.AreEqual(1, _help.GetXElements("TcpLink", true, "名称", "视觉程序").Count());
            Assert.AreEqual(1, _help.GetXElements("TcpLink", true, "名称", "MES系统").Count());

            Assert.AreEqual(1, _help.GetXElements("Data", true, "名称", "组装精度").Count());
            Assert.AreEqual(1, _help.GetXElements("Data", true, "名称", "组装位置").Count());
            Assert.AreEqual(1, _help.GetXElements("Data", true, "名称", "上传数据").Count());
        }

        [TestMethod]
        public void GetKeyValuesFromXElementAttributesTest()
        {
            InitXmlFile();

            var element = _help.GetXElements("Data", true, "名称", "组装精度").First();
            var dic = _help.GetKeyValuesFromXElementAttributes(element);
            Assert.AreEqual(3, dic.Count);

            var noAttributeElement = _help.GetXElements("TcpLink", false).First();
            var nullDic = _help.GetKeyValuesFromXElementAttributes(noAttributeElement);
            Assert.AreEqual(0, nullDic.Count);

            CheckArrayValueEqauls(dic.Keys.ToList(), "名称", "版本", "权限");
            CheckArrayValueEqauls(dic.Values.ToList(), "组装精度", "1.0", "2");
        }

        [TestMethod]
        public void UpdateXElementAttributeTest()
        {
            InitXmlFile();

            var element = _help.GetXElements("Data", true, "名称", "组装精度").First();
            _help.UpdateXElementAttribute(element, new Dictionary<string, string?> { { "版本", "2.0" }, { "权限", "3" } });
            element = _help.GetXElements("Data", true, "名称", "组装精度").First();
            var dic = _help.GetKeyValuesFromXElementAttributes(element);
            Assert.AreEqual(3, dic.Count);
            CheckArrayValueEqauls(dic.Keys.ToList(), "名称", "版本", "权限");
            CheckArrayValueEqauls(dic.Values.ToList(), "组装精度", "2.0", "3");

            _help.UpdateXElementAttribute(element, new Dictionary<string, string?> { { "是否显示", "true" } });
            element = _help.GetXElements("Data", true, "名称", "组装精度").First();
            dic = _help.GetKeyValuesFromXElementAttributes(element);
            Assert.AreEqual(4, dic.Count);
            CheckArrayValueEqauls(dic.Keys.ToList(), "名称", "版本", "权限", "是否显示");
            CheckArrayValueEqauls(dic.Values.ToList(), "组装精度", "2.0", "3", "true");

            _help.UpdateXElementAttribute(element, null);
            Assert.AreEqual(2, _help.GetXElements("Data", false).Count());
        }

        private void InitXmlFile()
        {
            _help = new XmlHelp(_fileName, "root");
            var xElementRoot = _help.GetXElements("root", false).First();

            var tcplinkNode = _help.CreateXElement("TcpLink", null);
            var tcplinkNodeChild = _help.CreateXElement("TcpLink", new Dictionary<string, string> { { "名称", "EPSON机器人" }, { "IP", "192.168.0.100" }, { "Port", "3000" }, { "超时时间", "3000" } });
            tcplinkNode.Add(tcplinkNodeChild);
            tcplinkNodeChild = _help.CreateXElement("TcpLink", new Dictionary<string, string> { { "名称", "视觉程序" }, { "IP", "192.168.10.100" }, { "Port", "10000" }, { "超时时间", "3000" } });
            tcplinkNode.Add(tcplinkNodeChild);
            tcplinkNodeChild = _help.CreateXElement("TcpLink", new Dictionary<string, string> { { "名称", "MES系统" }, { "IP", "127.0.0.1" }, { "Port", "80" }, { "超时时间", "3000" } });
            tcplinkNode.Add(tcplinkNodeChild);

            var dataNode = _help.CreateXElement("Data", null);
            var dataNodeChild = _help.CreateXElement("Data", new Dictionary<string, string> { { "名称", "组装精度" }, { "版本", "1.0" }, { "权限", "2" } });
            dataNode.Add(dataNodeChild);
            dataNodeChild = _help.CreateXElement("Data", new Dictionary<string, string> { { "名称", "组装位置" }, { "版本", "1.0" }, { "权限", "2" } });
            dataNode.Add(dataNodeChild);
            dataNodeChild = _help.CreateXElement("Data", new Dictionary<string, string> { { "名称", "上传数据" }, { "版本", "1.0" }, { "权限", "2" } });
            dataNode.Add(dataNodeChild);

            xElementRoot.Add(tcplinkNode);
            xElementRoot.Add(dataNode);
            _help.Save();
        }

        private void CheckArrayValueEqauls(List<string> source, params string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Assert.IsTrue(source.Contains(args[i]));
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