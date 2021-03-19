using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    sealed class RemarkAttribute:Attribute
    {
        private string _name;
        public RemarkAttribute(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }
    }

    public static class ObjectExtension
    {
        public static string Validate(this PropertyInfo prop)
        {
            string result = prop.Name;

            if (prop.IsDefined(typeof(RemarkAttribute)))
            {
               object[] oProp = prop.GetCustomAttributes(true);
                foreach (RemarkAttribute remarkAttribute in oProp)
                {
                    result = remarkAttribute.GetName();
                }
            }
            return result;
        }
    }
}
