using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDemo
{
    //注意此处的T可以改成任何非C#关键定，且可定义多个，where也可以是多个
    public class GenericClass<T, K, M> where T : People where K : People where M : People
    {
        public void Show(T tParam)
        {
            Console.WriteLine($"{tParam.Id}:{tParam.Name}.");
        }

        public void ShowTType(T tParam)
        {
            Console.WriteLine($"Type:{tParam.GetType()},Value:{tParam}.");
        }
    }
}
