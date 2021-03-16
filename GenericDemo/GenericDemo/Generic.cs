using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDemo
{
    public class Generic
    {
        public static void Show<T>(T tParam)
        {
            Console.WriteLine($"Type:{tParam.GetType()},Value:{tParam}.");
        }
    }
}
