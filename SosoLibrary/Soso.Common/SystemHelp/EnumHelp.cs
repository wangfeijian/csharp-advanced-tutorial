#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2024 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.SystemHelp
 * 唯一标识：ffa5ecce-f796-435f-8579-d3d5358cef5b
 * 文件名：EnumHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：2/2/2024 8:10:36 AM
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Soso.Common.SystemHelp
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class EnumHelp
    {
        /// <summary>
        /// 获取枚举项的字符串集合
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns></returns>
        public static IEnumerable<string> GetEnumContents<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// 获取枚举所有项的描述集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<string> GetEnumDescriptions<T>() where T : Enum
        {

            return Enum.GetValues(typeof(T)).Cast<Enum>().Select(x => x.GetDescription());
        }

        /// <summary>
        /// 获取枚举值的描述（需要添加[Description]特性）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;


        }

        /// <summary>
        /// 将描述转成枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static T EnumConvertByDescription<T>(string desc)
        {
            var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
            var field = fields.FirstOrDefault(w => (w.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description == desc);
            if (field == null)
                return default(T);
            return (T)Enum.Parse(typeof(T), field.Name);
        }

        /// <summary>
        /// 通过名称转换成枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumStr">枚举的名称</param>
        /// <returns></returns>
        public static T ConvertToEnum<T>(string enumStr)
        {
            var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);
            var field = fields.FirstOrDefault(w => w.Name == enumStr);
            if (field == null)
                return default(T);
            return (T)Enum.Parse(typeof(T), enumStr);
        }


        /// <summary>
        /// 将描述转成枚举
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static object EnumConvertByDescription(Type type, string desc)
        {
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            var field = fields.FirstOrDefault(w => (w.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description == desc);
            if (field == null)
                return null;
            return Enum.Parse(type, field.Name);
        }

    }
}
