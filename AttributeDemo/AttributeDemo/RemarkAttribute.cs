using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeDemo
{

    [AttributeUsage(AttributeTargets.Enum|AttributeTargets.Field)]
    sealed class RemarkAttribute : Attribute
    {
        private readonly string _name;
        public RemarkAttribute(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }
    }

    public static class EnumExtension
    {
        public static string GetRemarkValue(this Enum en)
        {
            Type type = en.GetType();
            FieldInfo field = type.GetField(en.ToString());
            if (field == null)
            {
                return en.ToString();
            }

            object[] attribute = field.GetCustomAttributes(typeof(RemarkAttribute), false);

            string name = en.ToString();

            foreach (RemarkAttribute remarkAttribute in attribute)
            {
                name = remarkAttribute.GetName();
            }
            return name;
        }
    }
}
