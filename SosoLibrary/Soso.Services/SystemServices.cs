#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：SosoMS.SingletonServices
 * 唯一标识：79569319-c552-4fb9-8d72-8baafe96ee90
 * 文件名：SystemServices
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：8/25/2023 8:51:49 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：王飞箭 wangfeijian
 * 时间：12/29/2023
 * 修改说明：
 * 1、增加系统参数的设置和获取方法。
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using CommunityToolkit.Mvvm.ComponentModel;
using Soso.Contract;
using Soso.Contract.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Soso.Services
{
    public delegate void SystemParameterHandle(string key, string oldValue, string newValue);
    public sealed partial class SystemServices : SingletonInstance<SystemServices>
    {
        private volatile bool _eventCanInvoke = true;
        public event SystemParameterHandle SystemParameterChangedEvent;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private SystemServices()
        {
            ConfigServices.Instance.InitSystemParameters();
            systemParameters = new ObservableCollection<SystemParameter>(ConfigServices.Instance.SystemParameters);
            foreach (var parameter in systemParameters)
            {
                parameter.ValueChanged += Parameter_ValueChanged;
            }
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [ObservableProperty]
        private ObservableCollection<SystemParameter> systemParameters;

        #region private method
        private void Parameter_ValueChanged(string key, string oldValue, string newValue)
        {
            ConfigServices.Instance.SaveSystemParameterXml(key, newValue);

            if (_eventCanInvoke)
            {
                SystemParameterChangedEvent?.BeginInvoke(key, oldValue, newValue, null, null);
            }

            _eventCanInvoke = true;
        }

        private void SetSystemParam<T>(string key, T value)
        {
            var systemParam = GetSystemParameter(key);

            _eventCanInvoke = false;
            systemParam.Value = value.ToString();

            List<string> errorMsg;
            if (!systemParam.ValueSetSucess(out errorMsg))
            {
                _eventCanInvoke = true;
                string msg = string.Join("\r\n", errorMsg);
                throw new ArgumentOutOfRangeException(msg);
            }
        }

        private T GetSystemParam<T>(string key, Func<string, bool> canConvert, Func<string, T> convertMethod)
        {
            var systemParam = GetSystemParameter(key);

            if (canConvert(systemParam.Value))
            {
                return convertMethod(systemParam.Value);
            }
            else
            {
                throw new InvalidCastException($"Parameter:{key} can't convert to {typeof(T)}");
            }
        }
        private SystemParameter GetSystemParameter(string key)
        {
            var systemParam = from parameter in SystemParameters
                              where parameter.Key == key
                              select parameter;

            if (systemParam == null || systemParam.Count() <= 0)
            {
                throw new KeyNotFoundException($"The key:{key} does not exist in the system parameter file");
            }

            return systemParam.First();
        }
        #endregion

        #region public method
        /// <summary>
        /// 更改系统参数描述语言，目前只支持中文和英文
        /// </summary>
        /// <param name="langType">0-中文；1-英文；</param>
        public void ChangeLanguage(int langType)
        {
            foreach (var parameter in SystemParameters)
            {
                parameter.LangType = langType;
            }
        }

        /// <summary>
        /// 获取<see cref="int"/>类型的参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns>返回<see cref="int"/>值</returns>
        /// <exception cref="InvalidCastException"/>
        /// <exception cref="KeyNotFoundException"/>
        public int GetIntSystemParam(string key)
        {
            return GetSystemParam(key,
                (strValue) =>
                {
                    int result;
                    return int.TryParse(strValue, out result);
                },
                (strValue) => { return int.Parse(strValue); });
        }

        /// <summary>
        /// 设置<see cref="int"/>类型的参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">值</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void SetIntSystemParam(string key, int value)
        {
            SetSystemParam(key, value);
        }

        /// <summary>
        /// 获取<see cref="double"/>类型的参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns>返回<see cref="double"/>值</returns>
        /// <exception cref="InvalidCastException"/>
        /// <exception cref="KeyNotFoundException"/>
        public double GetDoubleSystemParam(string key)
        {
            return GetSystemParam(key,
                (strValue) =>
                {
                    double result;
                    return double.TryParse(strValue, out result);
                },
                (strValue) => { return double.Parse(strValue); });
        }

        /// <summary>
        /// 设置<see cref="double"/>类型的参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">值</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void SetDoubleSystemParam(string key, double value)
        {
            SetSystemParam(key, value);
        }

        /// <summary>
        /// 获取<see cref="bool"/>类型的参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns>返回<see cref="bool"/>值</returns>
        /// <exception cref="InvalidCastException"/>
        /// <exception cref="KeyNotFoundException"/>
        public bool GetBoolSystemParam(string key)
        {
            return GetSystemParam(key,
                (strValue) =>
                {
                    int result;
                    return int.TryParse(strValue, out result);
                },
                (strValue) => { return int.Parse(strValue) != 0; });
        }

        /// <summary>
        /// 设置<see cref="bool"/>类型的参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">值</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void SetBoolSystemParam(string key, bool value)
        {
            int result = value ? 1 : 0;
            SetSystemParam(key, result);
        }

        /// <summary>
        /// 获取<see cref="string"/>类型的参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns>返回<see cref="string"/>值</returns>
        /// <exception cref="KeyNotFoundException"/>
        public string GetStringSystemParam(string key)
        {
            return GetSystemParam(key,
                (strValue) =>
                {
                    return true;
                },
                (strValue) => { return strValue; });
        }

        /// <summary>
        /// 设置<see cref="string"/>类型的参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">值</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void SetStringSystemParam(string key, string value)
        {
            SetSystemParam(key, value);
        }
        #endregion
    }
}
