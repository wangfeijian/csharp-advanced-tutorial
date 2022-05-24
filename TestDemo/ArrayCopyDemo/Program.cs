using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayCopyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] num1 = {1,2,3,4,5,6};
            int[] num2 = {7, 8, 9, 10, 11, 12};
            int[] num3 = {7, 8, 9, 10, 11, 12};
            int[] num4 = {7, 8, 9, 10, 11, 12};
            int[] num5 = new int[24];
            Array.ConstrainedCopy(num1, 0, num5, 0, num1.Length);
            Array.ConstrainedCopy(num2, 0, num5, 6, num2.Length);
            Array.ConstrainedCopy(num2, 0, num5, 12, num3.Length);
            Array.ConstrainedCopy(num2, 0, num5, 18, num4.Length);
            foreach (var i in num5)
            {
                Console.WriteLine(i);
            }

            string str;
            for (int i = 0; i < 7; i++)
            {
                str = "位移传感器" + i;
                Console.WriteLine(str);
            }
        }
    }
}
