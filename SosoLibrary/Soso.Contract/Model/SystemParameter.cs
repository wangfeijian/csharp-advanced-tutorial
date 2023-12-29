#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Contract.Model
 * 唯一标识：652d6fc7-a4ac-4b64-9a7b-c59dcff2e634
 * 文件名：SystemParameter
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:40:57 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：王飞箭 wangfeijian
 * 时间：2023/12/29
 * 修改说明：
 * 1、增加验证类GreaterThan和LessThan
 * 2、增加Value值在赋值时的验证
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Soso.Contract.Model
{
    public sealed class GreaterThanAttribute : ValidationAttribute
    {
        public GreaterThanAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance,
                otherValue = instance.GetType().GetProperty(PropertyName).GetValue(instance);

            double min, doubleValue;

            if (double.TryParse(value.ToString(), out doubleValue))
            {
                if (double.TryParse(otherValue.ToString(), out min))
                {
                    if (doubleValue >= min)
                    {
                        return ValidationResult.Success;
                    }

                    return new($"The current value:{doubleValue} is smaller than the min value{min}");
                }
            }
            return ValidationResult.Success;

        }
    }

    public sealed class LessThanAttribute : ValidationAttribute
    {
        public LessThanAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance,
                otherValue = instance.GetType().GetProperty(PropertyName).GetValue(instance);

            double max, doubleValue;

            if (double.TryParse(value.ToString(), out doubleValue))
            {
                if (double.TryParse(otherValue.ToString(), out max))
                {
                    if (doubleValue <= max)
                    {
                        return ValidationResult.Success;
                    }

                    return new($"The current value:{doubleValue} is bigger than the max value:{max}");
                }
            }
            return ValidationResult.Success;

        }
    }

    public delegate void ValueChangedHandler(string key, string oldValue, string newValue);

    public partial class SystemParameter : ObservableValidator
    {
        public event ValueChangedHandler ValueChanged;
        public string Key { get; private set; }

        public string Description { get; private set; }
        public string EnglishDescription { get; private set; }
        public string MaxValue { get; private set; }
        public string MinValue { get; private set; }
        public string Units { get; private set; }
        public int AuthorityLevel { get; private set; }

        [NotifyPropertyChangedFor(nameof(ShowDescription))]
        [ObservableProperty]
        private int langType;
        public string ShowDescription
        {
            get
            {
                return LangType == 0 ? Description : EnglishDescription;
            }
        }

        private IReadOnlyCollection<ValidationResult> _validationResults;
        private string _value;
        [GreaterThan(nameof(MinValue))]
        [LessThan(nameof(MaxValue))]
        public string Value
        {
            get => _value;
            set
            {
                string oldValue = _value;
                if (_value != value)
                {
                    if (TrySetProperty(ref _value, value, out _validationResults))
                    {
                        ValueChanged?.Invoke(Key, oldValue, value);
                    }
                }
            }
        }

        public bool ValueSetSucess(out List<string> msg)
        {
            msg = new List<string>();
            if (_validationResults != null && _validationResults.Count > 0)
            {
                foreach (ValidationResult validationResult in _validationResults)
                {
                    msg.Add(validationResult.ErrorMessage);
                }
                return false;
            }
            return true;
        }
    }
}
