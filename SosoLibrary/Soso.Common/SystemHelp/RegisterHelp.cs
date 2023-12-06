#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.SystemHelp
 * 唯一标识：af738a6f-98eb-4deb-965d-0db57c0688ec
 * 文件名：RegisterHelp
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/12/2023 2:18:43 PM
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


using Soso.Contract;
using System.Collections.Concurrent;

namespace Soso.Common.SystemHelp
{
    /// <summary>
    /// 寄存器帮助类
    /// </summary>
    /// <remarks>
    /// 该类定义了四种系统寄存器类型，分别为<see langword="bool"/>型、<see langword="int"/>型、<see langword="double"/>型、<see langword="string"/>型
    /// </remarks>
    public class RegisterHelp : SingletonInstance<RegisterHelp>
    {
        #region Delegate Event

        /// <summary>
        /// 寄存器值改变委托
        /// </summary>
        /// <remarks>
        /// 定义一个寄存器值改变的委托，利用这个委托可以监控寄存器的值是否改变
        /// </remarks>
        /// <typeparam name="T">寄存器类型</typeparam>
        /// <param name="index">索引</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        public delegate void RegisterChangedHandle<T>(int index, T oldValue, T newValue);

        /// <summary>
        /// <see langword="bool"/>类型寄存器值改变事件
        /// </summary>
        public event RegisterChangedHandle<bool> BoolRegisterChanged;

        /// <summary>
        /// <see langword="int"/>类型寄存器值改变事件
        /// </summary>
        public event RegisterChangedHandle<int> IntRegisterChanged;

        /// <summary>
        /// <see langword="double"/>类型寄存器值改变事件
        /// </summary>
        public event RegisterChangedHandle<double> DoubleRegisterChanged;

        /// <summary>
        /// <see langword="string"/>类型寄存器值改变事件
        /// </summary>
        public event RegisterChangedHandle<string> StringRegisterChanged;

        #endregion

        private ConcurrentDictionary<int, bool> _boolRegisters = new ConcurrentDictionary<int, bool>();
        private ConcurrentDictionary<int, int> _intRegisters = new ConcurrentDictionary<int, int>();
        private ConcurrentDictionary<int, double> _doubleRegisters = new ConcurrentDictionary<int, double>();
        private ConcurrentDictionary<int, string> _stringRegisters = new ConcurrentDictionary<int, string>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private RegisterHelp() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #region Public Method

        /// <summary>
        /// 通过索引获取<see langword="bool"/>型寄存器的值
        /// </summary>
        /// <remarks>
        /// 通过给出的索引，获取<see langword="bool"/>寄存器的值，如果寄存器不存在就默认设置为<see langword="false"/>
        /// </remarks>
        /// <param name="index">索引</param>
        /// <returns><see langword="bool"/>值</returns>
        public bool GetBoolRegister(int index)
        {
            return _boolRegisters.GetOrAdd(index, false);
        }

        /// <summary>
        /// 通过索引更新<see cref="bool"/>型寄存器的值
        /// </summary>
        /// <remarks>
        /// 通过给出的索引，更新<see cref="bool"/>型寄存器的值，如果索引存就更新，不存在就添加<br/>
        /// 可以决定寄存器更新是否利用事件广播，只有在新的值和老的值不一致且开启了事件通知才会触发相应事件
        /// </remarks>
        /// <param name="index">索引</param>
        /// <param name="value">更新值</param>
        /// <param name="notify">是否利用事件广播</param>
        public void SetBoolRegister(int index, bool value, bool notify = false)
        {
            bool oldValue = GetBoolRegister(index);

            _boolRegisters.AddOrUpdate(index, value, (oldKey, oldValue) => value);

            if (notify && oldValue != value)
            {
                BoolRegisterChanged?.Invoke(index, oldValue, value);
            }
        }

        /// <summary>
        /// 通过索引获取<see langword="int"/>型寄存器的值
        /// </summary>
        /// <remarks>
        /// 通过给出的索引，获取<see langword="int"/>寄存器的值，如果寄存器不存在就默认设置为0
        /// </remarks>
        /// <param name="index">索引</param>
        /// <returns><see langword="int"/>值</returns>
        public int GetIntRegister(int index)
        {
            return _intRegisters.GetOrAdd(index, 0);
        }

        /// <summary>
        /// 通过索引更新<see cref="int"/>型寄存器的值
        /// </summary>
        /// <remarks>
        /// 通过给出的索引，更新<see cref="int"/>型寄存器的值，如果索引存就更新，不存在就添加<br/>
        /// 可以决定寄存器更新是否利用事件广播，只有在新的值和老的值不一致且开启了事件通知才会触发相应事件
        /// </remarks>
        /// <param name="index">索引</param>
        /// <param name="value">更新值</param>
        /// <param name="notify">是否利用事件广播</param>
        public void SetIntRegister(int index, int value, bool notify = false)
        {
            int oldValue = GetIntRegister(index);

            _intRegisters.AddOrUpdate(index, value, (oldKey, oldValue) => value);

            if (notify && oldValue != value)
            {
                IntRegisterChanged?.Invoke(index, oldValue, value);
            }
        }

        /// <summary>
        /// 通过索引获取<see langword="double"/>型寄存器的值
        /// </summary>
        /// <remarks>
        /// 通过给出的索引，获取<see langword="double"/>寄存器的值，如果寄存器不存在就默认设置为0
        /// </remarks>
        /// <param name="index">索引</param>
        /// <returns><see langword="double"/>值</returns>
        public double GetDoubleRegister(int index)
        {
            return _doubleRegisters.GetOrAdd(index, 0);
        }

        /// <summary>
        /// 通过索引更新<see cref="double"/>型寄存器的值
        /// </summary>
        /// <remarks>
        /// 通过给出的索引，更新<see cref="double"/>型寄存器的值，如果索引存就更新，不存在就添加<br/>
        /// 可以决定寄存器更新是否利用事件广播，只有在新的值和老的值不一致且开启了事件通知才会触发相应事件
        /// </remarks>
        /// <param name="index">索引</param>
        /// <param name="value">更新值</param>
        /// <param name="notify">是否利用事件广播</param>
        public void SetDoubleRegister(int index, double value, bool notify = false)
        {
            double oldValue = GetDoubleRegister(index);

            _doubleRegisters.AddOrUpdate(index, value, (oldKey, oldValue) => value);

            if (notify && oldValue != value)
            {
                DoubleRegisterChanged?.Invoke(index, oldValue, value);
            }
        }

        /// <summary>
        /// 通过索引获取<see langword="string"/>型寄存器的值
        /// </summary>
        /// <remarks>
        /// 通过给出的索引，获取<see langword="string"/>寄存器的值，如果寄存器不存在就默认设置为空字符串
        /// </remarks>
        /// <param name="index">索引</param>
        /// <returns><see langword="string"/>值</returns>
        public string GetStringRegister(int index)
        {
            return _stringRegisters.GetOrAdd(index, "");
        }

        /// <summary>
        /// 通过索引更新<see cref="string"/>型寄存器的值
        /// </summary>
        /// <remarks>
        /// 通过给出的索引，更新<see cref="string"/>型寄存器的值，如果索引存就更新，不存在就添加<br/>
        /// 可以决定寄存器更新是否利用事件广播，只有在新的值和老的值不一致且开启了事件通知才会触发相应事件
        /// </remarks>
        /// <param name="index">索引</param>
        /// <param name="value">更新值</param>
        /// <param name="notify">是否利用事件广播</param>
        public void SetStringRegister(int index, string value, bool notify = false)
        {
            string oldValue = GetStringRegister(index);

            _stringRegisters.AddOrUpdate(index, value, (oldKey, oldValue) => value);

            if (notify && oldValue != value)
            {
                StringRegisterChanged?.Invoke(index, oldValue, value);
            }
        }

        #endregion

    }
}