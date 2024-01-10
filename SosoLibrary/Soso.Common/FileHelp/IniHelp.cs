#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.FileHelp
 * 唯一标识：265846ab-e801-415b-8186-f6b214ad3f9f
 * 文件名：IniHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：6/27/2023 1:01:40 PM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：王飞箭 wangfeijian
 * 时间：6/28/2023 10:41 AM
 * 修改说明：
 * 1、增加读写数组数据到INI文件中
 * 2、增加删除相关节点方法
 * 3、增加静态读取相关方法
 * 版本：V1.0.0
 * ----------------------------------------------------------------
 * 修改人：王飞箭 wangfeijian
 * 时间：6/29/2023 13：49
 * 修改说明：
 * 1、将创建文件的方法移到Base类中
 * 版本：V1.0.0
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

#nullable enable
using Newtonsoft.Json;
using System;
using System.Linq;
using Windows.Win32;

namespace Soso.Common.FileHelp
{
    /// <summary>
    /// Ini文件帮助类
    /// </summary>
    /// <remarks>
    /// 该类下面有静态方法和实例方法<br/>
    /// 在使用读写操作时，一定要使用相对的方法进行读写，否则有可能出现乱码。
    /// </remarks>
    public sealed class IniHelp
    {
        #region Field And Properties

        private string _iniFileName;
        private const uint BufferLength = (uint)short.MaxValue;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string IniFileName
        {
            get { return _iniFileName; }
            set { _iniFileName = value; }
        }

        #endregion

        #region Ctor Private Method

        /// <summary>
        /// 使用文件名进行构造
        /// </summary>
        /// <param name="iniFileName">文件全名</param>
        public IniHelp(string iniFileName)
        {
            _iniFileName = iniFileName;
            FileBaseHelp.CreateDirForFullName(iniFileName);
        }

        private static void CheckParamIsNotNull(string param)
        {
            string message = string.Format("Parameter {0} cannot be null!!", nameof(param));

            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), message);
            }
        }
        #endregion

        #region Instance Public Method
        /// <summary>
        /// 获取指定的节点下的键值
        /// </summary>
        /// <remarks>
        /// 通过提供的节点名称，获取该节点下某个键所对应的值，如果节点或者键不存在就返回默认值
        /// </remarks>
        /// <example>
        /// 示例数据
        /// <code lang="ini">
        /// <![CDATA[
        /// [Array]
        /// First=第一个
        /// Second = 第二个
        /// Third=第三个
        /// [Base]
        /// First = 第一个
        /// Second=第二个
        /// Third = 第三个
        /// ]]>
        /// </code>
        /// 使用方法
        /// <code>
        /// <![CDATA[
        /// var iniHelp = new IniHelp(fileName);
        /// // 返回空字符串
        /// var result = iniHelp.ReadString(null, null);
        /// 
        /// // 返回"hello"
        /// result = iniHelp.ReadString("", "First", "hello");
        /// 
        /// // 返回"第一个"
        /// result = iniHelp.ReadString("Array", "First");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="section">节点</param>
        /// <param name="key">键值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>获取的键值</returns>
        public string? ReadString(string? section, string? key, string? defaultValue = "")
        {
            if (string.IsNullOrWhiteSpace(section) || string.IsNullOrWhiteSpace(key))
            {
                return string.IsNullOrWhiteSpace(defaultValue) ? "" : defaultValue;
            }

            string value = defaultValue!;

            unsafe
            {
                fixed (char* tempStr = new char[BufferLength])
                {
                    var byteReturn = PInvoke.GetPrivateProfileString(section, key, defaultValue, tempStr, BufferLength, _iniFileName);

                    if (byteReturn != 0)
                    {
                        value = new string(tempStr);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// 往指定节点下写入键值
        /// </summary>
        /// <remarks>
        /// 在指定的节点下面指定的键中写入相应的值，如果不存在该节点或者该键就添加<br/>
        /// 如果输入的节点或者键为空，则返回<see langword="false"/>
        /// </remarks>
        /// <example>
        /// 示例数据
        /// <code lang="ini">
        /// <![CDATA[
        /// [Array]
        /// First=第一个
        /// Second = 第二个
        /// Third=第三个
        /// [Base]
        /// First = 第一个
        /// Second=第二个
        /// Third = 第三个
        /// ]]>
        /// </code>
        /// 使用方法
        /// <code>
        /// <![CDATA[
        /// var iniHelp = new IniHelp(fileName);
        /// // 下面这句代码将ini文件中Array节点下的First值改为111
        /// iniHelp.WriteString("Array", "First", "111");
        /// 
        /// // 下面这句代码将添加一个全新的节点和相关的键值对
        /// iniHelp.WriteString("New", "Key", "Value");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>返回写入是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public bool WriteString(string section, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(section) || string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            return PInvoke.WritePrivateProfileString(section, key, value, _iniFileName);
        }

        /// <summary>
        /// 往指定节点追加字符串
        /// </summary>
        /// <remarks>
        /// 在指定的节点下面指定的键中追加字符串到值中，如果不存在该节点或者该键就按追加值添加<br/>
        /// 如果输入的节点或者键为空，则返回<see langword="false"/>
        /// </remarks>
        /// <example>
        /// 示例数据
        /// <code lang="ini">
        /// <![CDATA[
        /// [Array]
        /// First=第一个
        /// Second = 第二个
        /// Third=第三个
        /// [Base]
        /// First = 第一个
        /// Second=第二个
        /// Third = 第三个
        /// ]]>
        /// </code>
        /// 使用方法
        /// <code>
        /// <![CDATA[
        /// var iniHelp = new IniHelp(fileName);
        /// // 下面的代码使得ini文件中First键变成：First=第一个123
        /// iniHelp.AppendString("Array", "First", "123");
        /// ]]>
        /// </code>
        /// </example>  
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="value">追加值</param>
        /// <returns>返回写入是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public bool AppendString(string section, string key, string value)
        {
            string newValue = ReadString(section, key) + value;

            return WriteString(section, key, newValue);
        }

        /// <summary>
        /// 向指定节点中写入对象实例的json字符串
        /// </summary>
        /// <remarks>
        /// 将传入的实例对象，通过<see cref="Newtonsoft"/>工具，将其序列化为Json字符串，再将Json字符串写入到相应的键中<br/>
        /// 注意：传入的对象避免为<see langword="string"/>的对象
        /// </remarks>
        /// <example>
        /// 将一个自定义的Person对象写到Ini文件中
        /// <code>
        /// <![CDATA[
        /// public record Person(string Name, int age);
        /// var person = new Person[] { new Person("wangfeijian", 100), new Person("wang", 300) });
        /// var iniHelp = new IniHelp(fileName);
        /// iniHelp.WriteObject("Object", "PersonArray", person);
        /// 
        /// // 结果如下：
        /// // [Object]
        /// // PersonArray=[{"Name":"wangfeijian","age":100},{"Name":"wang","age":300}]
        /// ]]>
        /// </code>
        /// </example>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="instance">对象实例</param>
        /// <returns>返回写入是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public bool WriteObject<T>(string section, string key, T instance)
        {
            return WriteString(section, key, JsonConvert.SerializeObject(instance));
        }

        /// <summary>
        /// 从文件中读取一个对象，并实例化
        /// </summary>
        /// <remarks>
        /// 如果是通过<seealso cref="WriteObject{T}(string, string, T)"/>方法来进行保存的序列化JSON对象<br/>
        /// 则可以通过该方法来进行反序列化，得到一个反序列化对象
        /// </remarks>
        /// <example>
        /// 数据源
        /// <code lang="ini">
        /// <![CDATA[
        /// [Object]
        /// PersonArray=[{"Name":"wangfeijian","age":100},{"Name":"wang","age":300}]
        /// ]]>
        /// </code>
        /// 获取一个Person数组对象
        /// <code>
        /// <![CDATA[
        /// var iniHelp = new IniHelp(fileName);
        /// Person[] persons = iniHelp.ReadObject<Person[]>("Object", "PersonArray");
        /// ]]>
        /// </code>
        /// </example>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <returns>对象实例</returns>
        public T? ReadObject<T>(string section, string key)
        {
            string str = ReadString(section, key)!;
            if (typeof(string) == typeof(T))
            {
                str = "\"" + str + "\"";
            }

            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// 获取所有的节点名称
        /// </summary>
        /// <remarks>
        /// 获取Ini文件里面所有的节点名称，并通过字符串数组的形式返回
        /// </remarks>
        /// <returns>返回一个<see langword="string"/>数组</returns>
        public string[] GetAllSectionName()
        {
            string value = string.Empty;

            unsafe
            {
                fixed (char* tempStr = new char[BufferLength])
                {
                    var byteReturn = PInvoke.GetPrivateProfileSectionNames(tempStr, BufferLength, _iniFileName);

                    if (byteReturn != 0)
                    {
                        value = new string(tempStr, 0, (int)byteReturn - 1);
                    }
                }
            }

            return value.Split('\0');
        }

        /// <summary>
        /// 判断节点是否存在
        /// </summary>
        /// <remarks>
        /// 判断Ini文件中是否存在输入的节点
        /// </remarks>
        /// <param name="section">节点名称</param>
        /// <returns>返回该节点是否存在，存在返回<see langword="true"/>, 不存在返回<see langword="false"/></returns>
        public bool IsSectionExist(string section)
        {
            var array = GetAllSectionName();

            return array.Contains(section);
        }

        /// <summary>
        /// 获取当前节点下所有的键
        /// </summary>
        /// <remarks>
        /// 获取Ini文件里面该节点下所有的键名称，并通过字符串数组的形式返回
        /// </remarks>
        /// <param name="section">节点名称</param>
        /// <returns>返回一个<see langword="string"/>数组</returns>
        public string[] GetKeysForSection(string section)
        {
            string value = string.Empty;

            unsafe
            {
                fixed (char* tempStr = new char[BufferLength])
                {
                    var byteReturn = PInvoke.GetPrivateProfileSection(section, tempStr, BufferLength, _iniFileName);

                    if (byteReturn != 0)
                    {

                        value = new string(tempStr, 0, (int)byteReturn - 1);
                    }
                }
            }

            return value.Split('\0').Select(item => item.Split('=').First()).ToArray();
        }

        /// <summary>
        /// 判断当前节点下是否存在此键
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键名称</param>
        /// <returns>存在返回<see langword="true"/>, 不存在返回<see langword="false"/></returns>
        public bool IsKeyExist(string section, string key)
        {
            var array = GetKeysForSection(section);

            return array.Contains(key);
        }

        /// <summary>
        /// 删除指定节点下的键
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键名称</param>
        /// <returns>删除成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public bool DeleteKey(string section, string key)
        {
            return PInvoke.WritePrivateProfileString(section, key, null, _iniFileName);
        }

        /// <summary>
        /// 删除节点下所有的键
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <returns>删除成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public bool DeleteAllKeys(string section)
        {
            foreach (var item in GetKeysForSection(section))
            {
                if (!DeleteKey(section, item))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Static Public Method

        /// <summary>
        /// 获取ini文件中所有节点的名称，通过文件名
        /// </summary>
        /// <param name="iniFileName">ini文件全路径</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns>返回一个<see langword="string"/>数组</returns>
        public static string[] GetAllSectionName(string iniFileName)
        {
            CheckParamIsNotNull(iniFileName);

            string value = string.Empty;

            unsafe
            {
                fixed (char* tempStr = new char[BufferLength])
                {
                    var byteReturn = PInvoke.GetPrivateProfileSectionNames(tempStr, BufferLength, iniFileName);

                    if (byteReturn != 0)
                    {
                        value = new string(tempStr, 0, (int)byteReturn - 1);
                    }
                }
            }

            return value.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries); 
        }

        /// <summary>
        /// 获取指定节点下所有的键值对组合
        /// </summary>
        /// <remarks>
        /// 通过ini文件及提供的节点名称，获取该节点下面所有的键值对组合，并以key=value的形式进行保存
        /// </remarks>
        /// <example>
        /// 示例数据
        /// <code lang="ini">
        /// <![CDATA[
        /// [Array]
        /// First=第一个
        /// Second = 第二个
        /// Third=第三个
        /// [Base]
        /// First = 第一个
        /// Second=第二个
        /// Third = 第三个
        /// ]]>
        /// </code>
        /// 使用方法
        /// <code>
        /// <![CDATA[
        /// var results = IniHelp.GetAllItems(fileName, "Array");
        /// // 返回如下：
        /// // ["First=第一个","Second=第二个","Third=第三个"]
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="iniFileName">ini文件路径</param>
        /// <param name="section">节点名称</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns>返回一个<see langword="string"/>数组</returns>
        public static string[] GetAllItems(string iniFileName, string section)
        {
            CheckParamIsNotNull(iniFileName);
            CheckParamIsNotNull(section);

            string value = string.Empty;

            unsafe
            {
                fixed (char* tempStr = new char[BufferLength])
                {
                    var byteReturn = PInvoke.GetPrivateProfileSection(section, tempStr, BufferLength, iniFileName);

                    if (byteReturn != 0)
                    {
                        value = new string(tempStr, 0, (int)byteReturn - 1);
                    }
                }
            }

            return value.Split('\0');
        }

        /// <summary>
        /// 往指定的节点中写入键值对
        /// </summary>
        /// <remarks>
        /// 直接往一个ini文件中的指定节点下，写入一个或多个键值对数据，如果节点不存在就创建该节点。
        /// </remarks>
        /// <example>
        /// 示例数据
        /// <code lang="ini">
        /// <![CDATA[
        /// [Array]
        /// First=第一个
        /// Second = 第二个
        /// Third=第三个
        /// [Base]
        /// First = 第一个
        /// Second=第二个
        /// Third = 第三个
        /// ]]>
        /// </code>
        /// 使用方法
        /// <code>
        /// <![CDATA[
        /// var result = IniHelp.SetItems(fileName, "Array", "Forth=第四个");
        /// // 上面语句执行成功后，ini文件中的数据如下：
        /// // [Array]
        /// // First=第一个
        /// // Second = 第二个
        /// // Third=第三个
        /// // Forth=第四个
        /// // [Base]
        /// // First = 第一个
        /// // Second=第二个
        /// // Third = 第三个
        /// ]]>
        /// </code>
        /// 写入多个数据
        /// <code>
        /// <![CDATA[
        /// var result = IniHelp.SetItems(fileName, "AddData", "Data1=DataOne", "Data2=DataTwo");
        /// // 上面语句执行成功后，ini文件中的数据如下：
        /// // [Array]
        /// // First=第一个
        /// // Second = 第二个
        /// // Third=第三个
        /// // Forth=第四个
        /// // [Base]
        /// // First = 第一个
        /// // Second=第二个
        /// // Third = 第三个
        /// // [AddData]
        /// // Data1=DataOne
        /// // Data2=DataTwo
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="iniFileName">文件名</param>
        /// <param name="section">节点名称</param>
        /// <param name="items">键值对</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns>写入成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public static bool SetItems(string iniFileName, string section, params string[] items)
        {
            CheckParamIsNotNull(iniFileName);
            CheckParamIsNotNull(section);

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return PInvoke.WritePrivateProfileSection(section, string.Join("\0", items), iniFileName);
        }

        /// <summary>
        /// 获取当前节点下所有的键
        /// </summary>
        /// <remarks>
        /// 获取Ini文件里面该节点下所有的键名称，并通过字符串数组的形式返回
        /// </remarks>
        /// <param name="iniFileName">文件路径</param>
        /// <param name="section">节点名</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns>返回一个<see langword="string"/>数组</returns>
        public static string[] GetSectionKeys(string iniFileName, string section)
        {
            CheckParamIsNotNull(iniFileName);
            CheckParamIsNotNull(section);

            string value = string.Empty;

            unsafe
            {
                fixed (char* tempStr = new char[BufferLength])
                {
                    var byteReturn = PInvoke.GetPrivateProfileString(section, null, null, tempStr, BufferLength, iniFileName);

                    if (byteReturn != 0)
                    {
                        value = new string(tempStr, 0, (int)byteReturn - 1);
                    }
                }
            }

            return value.Split('\0').Select(item => item.Split('=').First()).ToArray();
        }

        /// <summary>
        /// 设置指定节点下指定键的值
        /// </summary>
        /// <remarks>
        /// 在指定的节点下面指定的键中写入相应的值，如果不存在该节点或者该键就添加<br/>
        /// 如果输入的节点或者键为空，则抛出一个<see cref="ArgumentNullException"/>
        /// </remarks>
        /// <example>
        /// 示例数据
        /// <code lang="ini">
        /// <![CDATA[
        /// [Array]
        /// First=第一个
        /// Second = 第二个
        /// Third=第三个
        /// [Base]
        /// First = 第一个
        /// Second=第二个
        /// Third = 第三个
        /// ]]>
        /// </code>
        /// 使用方法
        /// <code>
        /// <![CDATA[
        /// // 下面这句代码将ini文件中Array节点下的First值改为111
        /// var result = IniHelp.SetValue(fileName, "Array", "First", "111");
        /// 
        /// // 下面这句代码将添加一个全新的节点和相关的键值对
        /// result = IniHelp.SetValue(fileName, "New", "Key", "Value");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="iniFileName">文件名</param>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns>返回写入是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public static bool SetValue(string iniFileName, string section, string key, string value)
        {
            CheckParamIsNotNull(iniFileName);
            CheckParamIsNotNull(section);
            CheckParamIsNotNull(key);
            CheckParamIsNotNull(value);

            return PInvoke.WritePrivateProfileString(section, key, value, iniFileName);
        }

        /// <summary>
        /// 获取指定节点下指定键的值
        /// </summary>
        /// <example>
        /// 示例数据
        /// <code lang="ini">
        /// <![CDATA[
        /// [Array]
        /// First=第一个
        /// Second = 第二个
        /// Third=第三个
        /// [Base]
        /// First = 第一个
        /// Second=第二个
        /// Third = 第三个
        /// ]]>
        /// </code>
        /// 使用方法
        /// <code>
        /// <![CDATA[
        /// // 抛出一个异常
        /// var result = IniHelp.GetValue(null, null, null);
        /// 
        /// // 返回"Hello"
        /// result = IniHelp.GetValue("", "Array", "First", "Hello");
        /// 
        /// // 返回"第一个"
        /// result = IniHelp.GetValue(fileName, "Array", "First");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="iniFileName">文件名</param>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="defautValue">默认值</param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns>获取的键值</returns>
        public static string GetValue(string iniFileName, string section, string key, string defautValue = "")
        {
            CheckParamIsNotNull(iniFileName);
            CheckParamIsNotNull(section);
            CheckParamIsNotNull(key);
            CheckParamIsNotNull(defautValue);

            string value = defautValue;

            unsafe
            {
                fixed (char* tempStr = new char[BufferLength])
                {
                    var byteReturn = PInvoke.GetPrivateProfileString(section, key, defautValue, tempStr, BufferLength, iniFileName);

                    if (byteReturn != 0)
                    {
                        value = new string(tempStr);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// 移除指定节点下的键
        /// </summary>
        /// <param name="iniFileName">文件路径</param>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>       
        /// <exception cref="ArgumentNullException"/>
        /// <returns>返回移除是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public static bool RemoveKey(string iniFileName, string section, string key)
        {
            CheckParamIsNotNull(iniFileName);
            CheckParamIsNotNull(section);
            CheckParamIsNotNull(key);

            return PInvoke.WritePrivateProfileString(section, key, null, iniFileName);
        }

        /// <summary>
        /// 移除指定的节点
        /// </summary>
        /// <remarks>
        /// 移除当前节点下所有的键值对，并将节点也移除
        /// </remarks>
        /// <param name="iniFileName">文件名</param>
        /// <param name="section">节点名</param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns>返回移除是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public static bool RemoveSection(string iniFileName, string section)
        {
            CheckParamIsNotNull(iniFileName);
            CheckParamIsNotNull(section);

            return PInvoke.WritePrivateProfileString(section, null, null, iniFileName);
        }

        /// <summary>
        /// 清空指定节点下的内容
        /// </summary>
        /// <remarks>
        /// 清空当前节点下所有的键值对，保留节点
        /// </remarks>
        /// <param name="iniFileName">文件名</param>
        /// <param name="section">节点名</param>
        /// <exception cref="ArgumentNullException"/>
        /// <returns>返回清空是否成功，成功返回<see langword="true"/>, 失败返回<see langword="false"/></returns>
        public static bool ClearSection(string iniFileName, string section)
        {
            CheckParamIsNotNull(iniFileName);
            CheckParamIsNotNull(section);

            return PInvoke.WritePrivateProfileSection(section, null, iniFileName);
        }
        #endregion
    }
}