#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common
 * 唯一标识：0ee02825-fe0a-4b91-b927-325238fd58b4
 * 文件名：SosoObject
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/6/2023 4:38:28 PM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：王飞箭 wangfeijian
 * 时间：7/14 2023
 * 修改说明：
 * 1、将重载及实现接口方法，继承基类文档
 * 版本：V1.0.0
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>


using System;
using System.Data;

namespace Soso.Common
{
    /// <summary>
    /// 框架自定义通用数据类型
    /// </summary>
    /// <remarks>
    /// 自定义的通用数据类型，可实现普通数据类型的获取及转换<br/>
    /// 通过该类可实现直接获取<see langword="int"/>、<see langword="bool"/>、<see langword="double"/>、<see langword="string"/>这些常见的数据
    /// </remarks>
    [Serializable]
    public class SosoObject : ICloneable, IComparable, IComparable<SosoObject>, IEquatable<SosoObject>
    {
        private object _value;

        #region Ctor

        /// <summary>
        /// 无参构造函数，得到的值为空
        /// </summary>
        public SosoObject()
        {
            _value = string.Empty;
        }

        /// <summary>
        /// <see langword="int"/>类型构造函数
        /// </summary>
        /// <param name="value"><see langword="int"/>类型的值</param>
        public SosoObject(int value)
        {
            _value = value;
        }

        /// <summary>
        /// <see langword="double"/>类型构造函数
        /// </summary>
        /// <param name="value"><see langword="double"/>类型的值</param>
        public SosoObject(double value)
        {
            _value = value;
        }

        /// <summary>
        /// <see langword="bool"/>类型构造函数
        /// </summary>
        /// <param name="value"><see langword="bool"/>类型的值</param>
        public SosoObject(bool value)
        {
            _value = value;
        }

        /// <summary>
        /// <see langword="string"/>类型构造函数
        /// </summary>
        /// <param name="value"><see langword="string"/>类型的值</param>
        public SosoObject(string value)
        {
            _value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 转换为<see langword="int"/>类型
        /// </summary>
        public int I
        {
            get
            {
                if (int.TryParse(S, out int value))
                {
                    return value;
                }

                return 0;
            }
        }

        /// <summary>
        /// 转换为<see langword="double"/>类型
        /// </summary>
        public double D
        {
            get
            {
                if (double.TryParse(S, out double value))
                {
                    return value;
                }

                return 0;
            }
        }

        /// <summary>
        /// 转换为<see langword="bool"/>类型
        /// </summary>
        public bool B
        {
            get
            {
                if (bool.TryParse(S, out bool value))
                {
                    return value;
                }

                return false;
            }
        }

        /// <summary>
        /// 转换为<see langword="string"/>类型
        /// </summary>
        public string S => _value.ToString()!;

        /// <summary>
        /// 是否为数字
        /// </summary>
        public bool IsNumber
        {
            get
            {
                return double.TryParse(S, out double _);
            }
        }

        /// <summary>
        /// 是否为空或者空白
        /// </summary>
        public bool IsNullOrEmpty
        {
            get
            {
                return _value == null ? true : string.IsNullOrEmpty(S);
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override string ToString() => S;

        /// <inheritdoc/>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">This instance is null</exception>
        /// <exception cref="ArgumentNullException">The other is null</exception>
        public int CompareTo(object? obj)
        {
            if (this is null)
            {
                throw new NullReferenceException();
            }

            if (obj == null)
            {
                throw new ArgumentNullException("Parameter is not SosoObject!!");
            }

            if (double.TryParse(obj.ToString(), out double value) && IsNumber)
            {
                return D.CompareTo(value);
            }

            return S.CompareTo(obj.ToString());
        }

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">This instance is null</exception>
        /// <exception cref="ArgumentNullException">The other is null</exception>
        public int CompareTo(SosoObject? other)
        {
            if (this is null)
            {
                throw new NullReferenceException();
            }

            if (other is null)
            {
                throw new ArgumentNullException("other");
            }

            if (other.IsNumber && IsNumber)
            {
                return D.CompareTo(other.D);
            }
            else
            {
                return S.CompareTo(other.S);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">This instance is null</exception>
        public bool Equals(SosoObject? other)
        {
            if (this is null)
            {
                throw new NullReferenceException();
            }

            if (other is null)
            {
                return false;
            }

            if (other.IsNumber && IsNumber)
            {
                return D.Equals(other.D);
            }
            else
            {
                return S.Equals(other.S);
            }
        }


        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">This instance is null</exception>
        /// <exception cref="ArgumentNullException">This instance is null</exception>
        public override bool Equals(object? obj)
        {
            if (this is null)
            {
                throw new NullReferenceException();
            }

            if (obj == null)
            {
                throw new ArgumentNullException("Parameter is not SosoObject!!");
            }

            if (double.TryParse(obj.ToString(), out double value) && IsNumber)
            {
                return D.Equals(value);
            }

            return S.Equals(obj.ToString());
        }

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException"></exception>
        public override int GetHashCode()
        {
            if (this is null)
            {
                throw new NullReferenceException();
            }

            return S.GetHashCode();
        }
        #endregion

        #region Implicit

        /// <summary>
        /// 将<see langword="int"/>数据类型隐式转换为<see cref="SosoObject"/>类型
        /// </summary>
        /// <param name="value"><see langword="int"/>值</param>
        /// <returns>新对象</returns>
        public static implicit operator SosoObject(int value)
        {
            return new SosoObject(value);
        }

        /// <summary>
        /// 将<see cref="SosoObject"/>类型隐式转换为<see langword="int"/>数据类型
        /// </summary>
        /// <param name="value"><see cref="SosoObject"/>对象</param>
        /// <returns><see langword="int"/>数据</returns>
        public static implicit operator int(SosoObject value)
        {
            return value.I;
        }

        /// <summary>
        /// 将<see langword="double"/>数据类型隐式转换为<see cref="SosoObject"/>类型
        /// </summary>
        /// <param name="value"><see langword="double"/>值</param>
        /// <returns>新对象</returns>
        public static implicit operator SosoObject(double value)
        {
            return new SosoObject(value);
        }

        /// <summary>
        /// 将<see cref="SosoObject"/>类型隐式转换为<see langword="double"/>数据类型
        /// </summary>
        /// <param name="value"><see cref="SosoObject"/>对象</param>
        /// <returns><see langword="double"/>数据</returns>
        public static implicit operator double(SosoObject value)
        {
            return value.D;
        }

        /// <summary>
        /// 将<see langword="bool"/>数据类型隐式转换为<see cref="SosoObject"/>类型
        /// </summary>
        /// <param name="value"><see langword="bool"/>值</param>
        /// <returns>新对象</returns>
        public static implicit operator SosoObject(bool value)
        {
            return new SosoObject(value);
        }

        /// <summary>
        /// 将<see cref="SosoObject"/>类型隐式转换为<see langword="bool"/>数据类型
        /// </summary>
        /// <param name="value"><see cref="SosoObject"/>对象</param>
        /// <returns><see langword="bool"/>数据</returns>
        public static implicit operator bool(SosoObject value)
        {
            return value.B;
        }

        /// <summary>
        /// 将<see langword="string"/>数据类型隐式转换为<see cref="SosoObject"/>类型
        /// </summary>
        /// <param name="value"><see langword="string"/>值</param>
        /// <returns>新对象</returns>
        public static implicit operator SosoObject(string value)
        {
            return new SosoObject(value);
        }

        /// <summary>
        /// 将<see cref="SosoObject"/>类型隐式转换为<see langword="string"/>数据类型
        /// </summary>
        /// <param name="value"><see cref="SosoObject"/>对象</param>
        /// <returns><see langword="string"/>数据</returns>
        public static implicit operator string(SosoObject value)
        {
            return value.S;
        }

        #endregion

        #region Operator Override + - * / > < == != >= <=

        #region + - * /

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象相加
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则将数字相加后，返回以相加结果为值的对象<br/>
        /// 如果有一个对象不是数字，则将两个对象的转换为字符串并连接起来，以连接起来的字符串作为该对象的值<br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>新对象</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static SosoObject operator +(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                return new SosoObject(soso1.S + soso2.S);
            }
            else
            {
                return new SosoObject(soso1.D + soso2.D);
            }
        }

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象相减
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则将数字相减后，返回以相减结果为值的对象<br/>
        /// 如果有一个对象不是数字，则抛出<see cref="InvalidExpressionException"/><br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>新对象</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        public static SosoObject operator -(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                throw new InvalidExpressionException("Two params msut be number!");
            }
            else
            {
                return new SosoObject(soso1.D - soso2.D);
            }
        }

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象相乘
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则将数字相乘后，返回以相乘结果为值的对象<br/>
        /// 如果有一个对象不是数字，则抛出<see cref="InvalidExpressionException"/><br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>新对象</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        public static SosoObject operator *(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                throw new InvalidExpressionException("Two params msut be number!");
            }
            else
            {
                return new SosoObject(soso1.D * soso2.D);
            }
        }

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象相除
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则将数字相除后，返回以相除结果为值的对象<br/>
        /// 如果有一个对象不是数字，则抛出<see cref="InvalidExpressionException"/><br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/><br/>
        /// 如果第二个对象为0，则抛出<see cref="DivideByZeroException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>新对象</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        /// <exception cref="DivideByZeroException"></exception>
        public static SosoObject operator /(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                throw new InvalidExpressionException("Two params msut be number!");
            }
            else
            {
                if (soso2.D == 0)
                {
                    throw new DivideByZeroException();
                }

                return new SosoObject(soso1.D / soso2.D);
            }
        }

        #endregion

        #region > < == != >= <=

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象进行大于比较
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则比较两个数字的大小<br/>
        /// 如果有一个对象不是数字，则抛出<see cref="InvalidExpressionException"/><br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>如果第一个对象大于第二个对象就返回<see langword="true"/>，否则就返回<see langword="false"/></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        public static bool operator >(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                throw new InvalidExpressionException("Two params msut be number!");
            }
            else
            {
                return soso1.D > soso2.D;
            }
        }

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象进行小于比较
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则比较两个数字的大小<br/>
        /// 如果有一个对象不是数字，则抛出<see cref="InvalidExpressionException"/><br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>如果第一个对象小于第二个对象就返回<see langword="true"/>，否则就返回<see langword="false"/></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        public static bool operator <(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                throw new InvalidExpressionException("Two params msut be number!");
            }
            else
            {
                return soso1.D < soso2.D;
            }
        }

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象进行大于等于比较
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则比较两个数字的大小<br/>
        /// 如果有一个对象不是数字，则抛出<see cref="InvalidExpressionException"/><br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>如果第一个对象大于等于第二个对象就返回<see langword="true"/>，否则就返回<see langword="false"/></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        public static bool operator >=(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                throw new InvalidExpressionException("Two params msut be number!");
            }
            else
            {
                return soso1.D >= soso2.D;
            }
        }

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象进行小于等于比较
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则比较两个数字的大小<br/>
        /// 如果有一个对象不是数字，则抛出<see cref="InvalidExpressionException"/><br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>如果第一个对象小于等于第二个对象就返回<see langword="true"/>，否则就返回<see langword="false"/></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        public static bool operator <=(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                throw new InvalidExpressionException("Two params msut be number!");
            }
            else
            {
                return soso1.D <= soso2.D;
            }
        }

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象进行等于比较
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则比较两个数字是否相等<br/>
        /// 如果有一个对象不是数字，则比较两个对象的<see cref="S"/>属性是否相等<br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>如果第一个对象等于第二个对象就返回<see langword="true"/>，否则就返回<see langword="false"/></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static bool operator ==(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                return soso1.S == soso2.S;
            }
            else
            {
                return soso1.D == soso2.D;
            }
        }

        /// <summary>
        /// 两个<see cref="SosoObject"/>对象进行不等于比较
        /// </summary>
        /// <remarks>
        /// 如果两个对象都是数字，则比较两个数字是否不相等<br/>
        /// 如果有一个对象不是数字，则比较两个对象的<see cref="S"/>属性是否不相等<br/>
        /// 如果任意一个对象为<see langword="null"/>，则抛出<see cref="NullReferenceException"/>
        /// </remarks>
        /// <param name="soso1">第一个对象</param>
        /// <param name="soso2">第二个对象</param>
        /// <returns>如果第一个对象不等于第二个对象就返回<see langword="true"/>，否则就返回<see langword="false"/></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static bool operator !=(SosoObject soso1, SosoObject soso2)
        {
            if (soso1 is null || soso2 is null)
            {
                throw new NullReferenceException("Parameter is null!!");
            }

            if (!soso1.IsNumber || !soso2.IsNumber)
            {
                return soso1.S != soso2.S;
            }
            else
            {
                return soso1.D != soso2.D;
            }
        }

        #endregion
        #endregion
    }
}