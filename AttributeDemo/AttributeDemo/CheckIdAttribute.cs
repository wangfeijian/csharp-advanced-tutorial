using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeDemo
{
    [AttributeUsage(AttributeTargets.Property)]
    sealed class CheckIdAttribute : CheckAttribute
    {

        public CheckIdAttribute(long min, long max)
        {
            Min = min;
            Max = max;
        }

        public long Min { get; set; }
        public long Max { get; set; }

        public override bool Check(object obj)
        {
            long value;
            if (!long.TryParse(obj.ToString(), out value))
                return false;

            return value > Min && value < Max;

        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    sealed class CheckStringAttribute : CheckAttribute
    {
        public CheckStringAttribute()
        {
        }

        public override bool Check(object obj)
        {
            if (string.IsNullOrEmpty(obj.ToString().Trim()))
            {
                return false;
            }

            return true;
        }
    }

    public abstract class CheckAttribute : Attribute
    {
        public string ErrorInfo { get; set; }
        public abstract bool Check(object obj);
    }
}
