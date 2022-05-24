using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HashSetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var array = new[] { 2, 31, 3, 5, 2, 4, 1, 5, 3, 32, 6, 2, 4, 3 };
            var hash = new HashSet<int>();

            foreach (var i in array)
            {
                hash.Add(array[i]);
            }

            Console.WriteLine("Array's count is {0}.", array.Length);
            Console.WriteLine("Hashset's count is {0}.", hash.Count);
            // HashSet类是指创建一个不包括重复元素的集合
            var hashSet = new HashSet<string>();
            hashSet.Add("apple");
            hashSet.Add("orange");
            hashSet.Add("apple");

            //添加两个apple后，里面其实只有一个apple
            foreach (var item in hashSet)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine(hashSet.Count);
            var t = new Test();
            t.ProTest = 10;
            Console.WriteLine(t.ProTest);
            Console.ReadLine();
        }
    }

    public class Test
    {
        public int ProTest { get; internal set; } = -1;


    }

}
