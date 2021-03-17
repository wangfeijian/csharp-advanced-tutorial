using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeDemo
{
    public class Manage
    {
        public static string GetInfo(People people)
        {
            Type type = people.GetType();

            string info = string.Empty;

            object[] attribute = type.GetCustomAttributes(typeof(InfoAttribute), false);

            foreach (InfoAttribute item in attribute)
            {
                info = item.GetInfo();
            }

            return info;
        }

        public static bool CheckValue(People p, out string msg)
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
                        if (!checkAttribute.Check(property.GetValue(p)))
                        {
                            msg = checkAttribute.ErrorInfo;
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
