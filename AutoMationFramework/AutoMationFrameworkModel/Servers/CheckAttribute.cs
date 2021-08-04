/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-29                               *
*                                                                    *
*           ModifyTime:     2021-07-29                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Check value attribute class              *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMationFrameworkModel.Servers
{
    public abstract class CheckAttribute : Attribute
    {
        //public string ErrorInfo { get; set; }
        public abstract bool Check(object obj, string strMin, string strMax);
    }

    [AttributeUsage(AttributeTargets.Property)]
    sealed class CheckValueAttribute : CheckAttribute
    {

        public CheckValueAttribute()
        {
        }

        public override bool Check(object obj, string strMin, string strMax)
        {
            double value;

            if (strMin == string.Empty || strMax == string.Empty || strMin == null || strMax == null) return true;

            if (!double.TryParse(obj.ToString(), out value))
                return false;

            double min = Convert.ToDouble(strMin);
            double max = Convert.ToDouble(strMax);

            return value >= min && value <= max;

        }
    }
}
