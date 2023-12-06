#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.SystemHelp
 * 唯一标识：346e9ffb-156f-4b49-93ee-5a1d3a07f4ca
 * 文件名：SerializationHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/6/2023 2:28:14 PM
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
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Soso.Common.SystemHelp
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    /// <remarks>
    /// 将实体对象进行各种序列化反序列化操作，对实例对象进行深拷贝
    /// </remarks>
    public static class SerializationHelp
    {
        /// <summary>
        /// 将对象序列化到文件中
        /// </summary>
        /// <remarks>
        /// 使用<see cref="DataContractSerializer"/>将对象实例序列化到文件中，如果文件存在则会删除原来文件
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="fileName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void DataContractSerialize<T>(T instance, string fileName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("The input object cannot be null!");
            }

            FileBaseHelp.CreateDirForFullName(fileName);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            try
            {
                using FileStream fileStream = File.Create(fileName);
                var serial = new DataContractSerializer(typeof(T));
                serial.WriteObject(fileStream, instance);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 将文件反序列化为一个对象
        /// </summary>
        /// <remarks>
        /// 使用<see cref="DataContractSerializer"/>将序列化的文件反序列化成一个对象
        /// </remarks>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="fileName">文件路径</param>
        /// <returns>成功返回一个<typeparamref name="T"/>对象，失败抛出<see cref="SerializationException"/></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="SerializationException"></exception>
        public static T DataContractDeserialize<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"Not found {fileName} file！");
            }

            try
            {
                using FileStream fileStream = File.OpenRead(fileName);
                using var reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas());
                var serial = new DataContractSerializer(typeof(T));
                var result = serial.ReadObject(reader, true);

                if (result == null)
                {
                    throw new SerializationException("Serialization error!");
                }
                else
                {
                    return (T)result;
                }
            }
            catch (Exception)
            {
                throw new SerializationException("Serialization error!");
            }

        }

        /// <summary>
        /// 使用<see cref="XmlSerializer"/>将对象深拷贝
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="instance">对象实例</param>
        /// <returns>成功返回一个<typeparamref name="T"/>对象，失败抛出<see cref="SerializationException"/></returns>
        /// <exception cref="SerializationException"></exception>
        public static T DeepCopyByXmlSerializer<T>(T instance) where T : new()
        {
            object? result;

            try
            {
                using MemoryStream stream = new MemoryStream();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, instance);
                stream.Seek(0, SeekOrigin.Begin);
                result = xmlSerializer.Deserialize(stream);

                if (result == null)
                {
                    throw new SerializationException("Serialization error!");
                }
                else
                {
                    return (T)result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}