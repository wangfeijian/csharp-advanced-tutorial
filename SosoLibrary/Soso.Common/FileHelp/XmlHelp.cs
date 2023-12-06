#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.FileHelp
 * 唯一标识：182b41a0-8183-429b-bf7f-03a4b690bfd5
 * 文件名：XmlHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/29/2023 11:09:29 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：王飞箭 wangfeijian
 * 时间：7/4/2023
 * 修改说明：
 * 1、增加更新XElement中的数据
 * 版本：V1.0.0
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Soso.Common.FileHelp
{
    /// <summary>
    /// XML文件帮助类   
    /// </summary>
    /// <remarks>
    /// 使用<see cref="XDocument"/>对一个存在的XML文件进行读写，并加载到XML文件树中<br/>
    /// 读取出来的所有节点都为<see cref="XElement"/>对象<br/>
    /// 使用<see cref="XDocument"/>生成一个XML文件，并保存到相应的路径
    /// </remarks>
    /// <seealso cref="System.Xml.Linq"/>
    public sealed class XmlHelp
    {
        #region Field
        private readonly string _fileName;
        private readonly XDocument _document;
        private readonly bool _isCreateNew = false;
        #endregion

        #region Ctor

        /// <summary>
        /// 以文件名构造，加载一个现有的xml文件<br/>
        /// 如果文件不存在，就会抛出一个异常
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="FileLoadException"/>
        public XmlHelp(string fileName)
        {
            _fileName = fileName;
            _document = CreateXDocumentInstance();
        }

        /// <summary>
        /// 以文件名和根节点名称构造，如果文件存在，会直接加载此文件<br/>
        /// 如果文件不存在，会建立以rootStr为根节点的xml文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="rootStr">根节点名称</param>
        /// <exception cref="FileLoadException"/>
        public XmlHelp(string fileName, string rootStr)
        {
            _fileName = fileName;
            _isCreateNew = true;
            _document = CreateXDocumentInstance(rootStr);
        }

        #endregion

        #region Private Method
        private XDocument CreateXDocumentInstance(string rootStr = "root")
        {
            if (!File.Exists(_fileName))
            {
                if (_isCreateNew)
                {
                    FileBaseHelp.CreateDirForFullName(_fileName);
                    var xmlDoc = new XDocument();
                    var root = new XElement(rootStr);
                    xmlDoc.Add(root);
                    xmlDoc.Save(_fileName);
                }
                else
                {
                    throw new FileNotFoundException($"Not found {_fileName} file！");
                }
            }

            try
            {
                return XDocument.Load(_fileName);
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Load file format error!\n" + ex.Message);
            }
        }

        private void CheckParamIsNotNullOrWhiteSpace(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentNullException(nameof(param), $"Parameter {nameof(param)} cannot be null or white space!!");
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 保存到文件
        /// </summary>
        public void Save()
        {
            _document.Save(_fileName);
        }

        /// <summary>
        /// 创建一个<see cref="XElement"/>对象
        /// </summary>
        /// <remarks>
        /// 使用指定的节点名，创建一个<see cref="XElement"/>对象，如果指定了attributes字典对象<br/>则添加相应的特性到节点中。<br/>
        /// 如果节点名称传入空白或者为空，则抛出一个<see cref="ArgumentNullException"/>
        /// </remarks>
        /// <example>
        /// 创建一个没有特性的节点
        /// <code>
        /// <![CDATA[
        /// var xmlHelp = new XmlHelp(fileName);
        /// var xElementRoot = xmlHelp.GetXElements("root", false).First();
        /// var xElementChild = xmlHelp.CreateXElement("Data", null);
        /// xElementRoot.Add(xElementChild);
        /// xmlHelp.Save();
        /// ]]>
        /// </code>
        /// 创建一个有特性的节点
        /// <code>
        /// <![CDATA[
        /// var xmlHelp = new XmlHelp(fileName);
        /// var xElementRoot = xmlHelp.GetXElements("root", false).First();
        /// var xElementChild = xmlHelp.CreateXElement("Data", new Dictionary<string, string> { { "First", "one" }, { "Second", "two" } });
        /// xElementRoot.Add(xElementChild);
        /// xmlHelp.Save();
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="name">XElement名称</param>
        /// <param name="attributes">特性字典</param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns><see cref="XElement"/>对象</returns>
        /// <seealso cref="XElement.SetAttributeValue(XName, object?)"/>
        public XElement CreateXElement(string name, Dictionary<string, string>? attributes = null)
        {
            CheckParamIsNotNullOrWhiteSpace(name);

            var element = new XElement(name);

            if (attributes != null && attributes.Count > 0)
            {
                foreach (var attr in attributes)
                {
                    element.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            return element;
        }

        /// <summary>
        /// 获取<see cref="XElement"/>集合
        /// </summary>
        /// <remarks>
        /// 使用指定的节点名获取相应的节点，如果不通过特性筛选，则返回所有指定节点名称的<see cref="XElement"/>对象<br/>
        /// 例如：如果一个节点中嵌套了一个同名的节点，则会返回两个<see cref="XElement"/>对象<br/>
        /// 如果使用特性筛选，则必须指定相应的特性名称和特性值，不指定就会抛出<see cref="ArgumentNullException"/><br/>
        /// 如果存在相同的特性名称的值，则返回多个<see cref="XElement"/>对象。<br/>注意：即使筛选出来为一个对象，也是一个集合。
        /// 如果想删除获取到的节点，直接调用该<see cref="XElement"/>对象中的<see cref="XNode.Remove()"/>方法即可
        /// </remarks>
        /// <example>
        /// 数据源
        /// <code lang="XML">
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <root>
        ///   <TcpLink>
        ///     <TcpLink 名称 = "EPSON机器人" IP="192.168.0.100" Port="3000" 超时时间="3000" />
        ///     <TcpLink 名称 = "视觉程序" IP="192.168.10.100" Port="10000" 超时时间="3000" />
        ///     <TcpLink 名称 = "MES系统" IP="127.0.0.1" Port="80" 超时时间="3000" />
        ///   </TcpLink>
        ///   <Data>
        ///     <Data 名称 = "组装精度" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "组装位置" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "上传数据" 版本="1.0" 权限="2" />
        ///   </Data>
        /// </root>
        /// ]]>
        /// </code>
        /// 使用上面的数据源进行没有特性节点的获取
        /// <code>
        /// <![CDATA[
        /// var xmlHelp = new XmlHelp(fileName);
        /// var xelements = xmlHelp.GetXElements("TcpLink", false);
        /// // xelements中有一个嵌套了TcpLink节点的对象集合 结果如下：
        /// //   <TcpLink>
        /// //     <TcpLink 名称 = "EPSON机器人" IP="192.168.0.100" Port="3000" 超时时间="3000" />
        /// //     <TcpLink 名称 = "视觉程序" IP="192.168.10.100" Port="10000" 超时时间="3000" />
        /// //     <TcpLink 名称 = "MES系统" IP="127.0.0.1" Port="80" 超时时间="3000" />
        /// //   </TcpLink>
        /// ]]>
        /// </code>
        /// 使用上面的数据源进行有特性节点的获取
        /// <code>
        /// <![CDATA[
        /// var element = xmlHelp.GetXElements("TcpLink", true, "名称", "EPSON机器人");
        /// // element中有一个TcpLink节点对象集合，结果如下：
        /// // <TcpLink 名称 = "EPSON机器人" IP="192.168.0.100" Port="3000" 超时时间="3000" />
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="elementName">节点名称</param>
        /// <param name="hasAttribute">是否存在特性</param>
        /// <param name="attributeName">特性名称</param>
        /// <param name="attributeValue">特性值</param>
        /// <exception cref="System.ArgumentNullException"/>
        /// <returns><see cref="IEnumerable{XElement}"/>集合对象</returns>
        /// <seealso cref="XContainer.Descendants()"/>
        public IEnumerable<XElement> GetXElements(string elementName, bool hasAttribute, string attributeName = "", string attributeValue = "")
        {
            CheckParamIsNotNullOrWhiteSpace(elementName);

            if (!hasAttribute)
            {
                return _document.Descendants(elementName).Where(item => !item.HasAttributes);
            }

            CheckParamIsNotNullOrWhiteSpace(attributeName);
            CheckParamIsNotNullOrWhiteSpace(attributeValue);

            return from element in _document.Descendants(elementName)
                   where element.HasAttributes && element.Attribute(attributeName) != null && element.Attribute(attributeName)!.Value == attributeValue
                   select element;
        }

        /// <summary>
        /// 获取<see cref="XElement"/>节点中所有特性的键和值
        /// </summary>
        /// <remarks>
        /// 通过一个<see cref="XElement"/>对象，获取对象中所有特性的键值对，如果没有特性就返回空
        /// </remarks>
        /// <example>
        /// 数据源
        /// <code lang="XML">
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <root>
        ///   <TcpLink>
        ///     <TcpLink 名称 = "EPSON机器人" IP="192.168.0.100" Port="3000" 超时时间="3000" />
        ///     <TcpLink 名称 = "视觉程序" IP="192.168.10.100" Port="10000" 超时时间="3000" />
        ///     <TcpLink 名称 = "MES系统" IP="127.0.0.1" Port="80" 超时时间="3000" />
        ///   </TcpLink>
        ///   <Data>
        ///     <Data 名称 = "组装精度" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "组装位置" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "上传数据" 版本="1.0" 权限="2" />
        ///   </Data>
        /// </root>
        /// ]]>
        /// </code>
        /// 使用上面的数据源进行获取
        /// <code>
        /// <![CDATA[
        /// var element = xmlHelp.GetXElements("TcpLink", true, "名称", "EPSON机器人");
        /// var keyValues = xmlHelp.GetKeyValuesFromXElementAttributes(element);
        /// // 结果如下：
        /// // {{名称, EPSON机器人},{IP, 192.168.0.100},{Port, 3000},{超时时间, 3000}}
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="element">节点</param>
        /// <returns>特性键值对</returns>
        /// <seealso cref="XElement.Attributes()"/>
        public Dictionary<string, string> GetKeyValuesFromXElementAttributes(XElement element)
        {
            return element.Attributes().ToDictionary(key => key.Name.ToString(), value => value.Value);
        }

        /// <summary>
        /// 使用特性字典更新<see cref="XElement"/>对象中的特性值
        /// </summary>
        /// <remarks>
        /// 使用特性字典更新<see cref="XElement"/>对象中特性的值，如果对象中存在此字典中的值就更新，否则就增加<br/>
        /// 如果传入的特性字典为空，则将该<see cref="XElement"/>对象中的特性清空
        /// </remarks>
        /// <example>
        /// 数据源
        /// <code lang="XML">
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <root>
        ///   <TcpLink>
        ///     <TcpLink 名称 = "EPSON机器人" IP="192.168.0.100" Port="3000" 超时时间="3000" />
        ///     <TcpLink 名称 = "视觉程序" IP="192.168.10.100" Port="10000" 超时时间="3000" />
        ///     <TcpLink 名称 = "MES系统" IP="127.0.0.1" Port="80" 超时时间="3000" />
        ///   </TcpLink>
        ///   <Data>
        ///     <Data 名称 = "组装精度" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "组装位置" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "上传数据" 版本="1.0" 权限="2" />
        ///   </Data>
        /// </root>
        /// ]]>
        /// </code>
        /// 更新已有特性
        /// <code>
        /// <![CDATA[
        /// // 先获取这个XElement对象
        /// var element = _help.GetXElements("Data", true, "名称", "组装精度").First();
        /// _help.UpdateXElementAttribute(element, new Dictionary<string, string?> { { "版本", "2.0" }, { "权限", "3" } });
        /// ]]>
        /// </code>
        /// 结果
        /// <code lang="XML">
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <root>
        ///   <TcpLink>
        ///     <TcpLink 名称 = "EPSON机器人" IP="192.168.0.100" Port="3000" 超时时间="3000" />
        ///     <TcpLink 名称 = "视觉程序" IP="192.168.10.100" Port="10000" 超时时间="3000" />
        ///     <TcpLink 名称 = "MES系统" IP="127.0.0.1" Port="80" 超时时间="3000" />
        ///   </TcpLink>
        ///   <Data>
        ///     <Data 名称 = "组装精度" 版本="2.0" 权限="3" />
        ///     <Data 名称 = "组装位置" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "上传数据" 版本="1.0" 权限="2" />
        ///   </Data>
        /// </root>
        /// ]]>
        /// </code>
        /// 新增特性
        /// <code>
        /// <![CDATA[
        /// var element = _help.GetXElements("Data", true, "名称", "组装精度").First();
        /// _help.UpdateXElementAttribute(element, new Dictionary<string, string?> { { "是否显示", "true" } });
        /// ]]>
        /// </code>
        /// 结果
        /// <code lang="XML">
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <root>
        ///   <TcpLink>
        ///     <TcpLink 名称 = "EPSON机器人" IP="192.168.0.100" Port="3000" 超时时间="3000" />
        ///     <TcpLink 名称 = "视觉程序" IP="192.168.10.100" Port="10000" 超时时间="3000" />
        ///     <TcpLink 名称 = "MES系统" IP="127.0.0.1" Port="80" 超时时间="3000" />
        ///   </TcpLink>
        ///   <Data>
        ///     <Data 名称 = "组装精度" 版本="2.0" 权限="3" 是否显示="true"/>
        ///     <Data 名称 = "组装位置" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "上传数据" 版本="1.0" 权限="2" />
        ///   </Data>
        /// </root>
        /// ]]>
        /// </code>
        /// 清空特性
        /// <code>
        /// <![CDATA[
        /// var element = _help.GetXElements("Data", true, "名称", "组装精度").First();
        /// _help.UpdateXElementAttribute(element, null);
        /// ]]>
        /// </code>
        /// 结果
        /// <code lang="XML">
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <root>
        ///   <TcpLink>
        ///     <TcpLink 名称 = "EPSON机器人" IP="192.168.0.100" Port="3000" 超时时间="3000" />
        ///     <TcpLink 名称 = "视觉程序" IP="192.168.10.100" Port="10000" 超时时间="3000" />
        ///     <TcpLink 名称 = "MES系统" IP="127.0.0.1" Port="80" 超时时间="3000" />
        ///   </TcpLink>
        ///   <Data>
        ///     <Data />
        ///     <Data 名称 = "组装位置" 版本="1.0" 权限="2" />
        ///     <Data 名称 = "上传数据" 版本="1.0" 权限="2" />
        ///   </Data>
        /// </root>
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="element"><see cref="XElement"/>对象</param>
        /// <param name="attributes">特性字典</param>
        /// <seealso cref="XElement.SetAttributeValue(XName, object?)"/>
        /// <seealso cref="XElement.RemoveAll()"/>
        public void UpdateXElementAttribute(XElement element, Dictionary<string, string?>? attributes)
        {
            if (attributes == null)
            {
                element.RemoveAll();
            }
            else if (attributes!.Count > 0)
            {
                foreach (var attr in attributes)
                {
                    element.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            Save();
        }

        #endregion
    }
}