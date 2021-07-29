/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-29                               *
*                                                                    *
*           ModifyTime:     2021-07-29                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Attribute manager class                  *
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonTools.Model;
using CommonTools.Tools;

namespace CommonTools.Servers
{
    public class AttributeManager
    {
        public static bool CheckValue(ParamInfo p, out string msg)
        {
            Type type = p.GetType();

            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo property in propertyInfos)
            {
                object[] objects = property.GetCustomAttributes(true);
                foreach (object o in objects)
                {
                    var checkAttribute = o as CheckAttribute;
                    if (checkAttribute != null)
                    {
                        if (!checkAttribute.Check(property.GetValue(p), p.MinValue, p.MaxValue))
                        {
                            string info = LocationServices.GetLang("ParamMessageInfo");
                            msg = string.Format(info, p.KeyValue);
                            return false;
                        }
                    }
                }
            }

            msg = string.Empty;
            return true;
        }
    }
}
