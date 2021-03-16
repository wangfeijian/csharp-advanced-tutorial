using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDemo
{
    public class Common
    {
        public static void ShowInt(int intParam)
        {
            Console.WriteLine($"Type:{intParam.GetType()},Value:{intParam}.");
        }

        public static void ShowString(string strParam)
        {
            Console.WriteLine($"Type:{strParam.GetType()},Value:{strParam}.");
        }

        public static void ShowDatetime(DateTime datetimeParam)
        {
            Console.WriteLine($"Type:{datetimeParam.GetType()},Value:{datetimeParam}.");
        }

        public static void ShowObject(object objectParam)
        {
            Console.WriteLine($"Type:{objectParam.GetType()},Value:{objectParam}.");
        }
    }
}
