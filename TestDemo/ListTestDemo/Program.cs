using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListTestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> oldList = new List<int>();
            List<int> newList = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                oldList.Add(i);
            }

            foreach (var i in oldList)
            {
                newList.Add(i);
            }
            oldList.Remove(0);
            foreach (var i in newList)
            {
                Console.WriteLine(i);
            }

            Console.WriteLine("--------------");
            foreach (var i in oldList)
            {
                Console.WriteLine(i);
            }
        }
    }
}
