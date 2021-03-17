using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttributeDemo
{
    sealed class InfoAttribute:Attribute
    {
        private readonly string _infomation;
        public InfoAttribute(string infomation)
        {
            _infomation = infomation;
        }

        public string GetInfo()
        {
            return _infomation;
        }
    }
}
