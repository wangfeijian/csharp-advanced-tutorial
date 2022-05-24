using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
           Assembly ass =  Assembly.Load("STLib");
           Type[] types =  ass.GetTypes();
            foreach (var type in types)
            {
                Console.WriteLine(type.FullName);
                Console.ReadLine();
            }

        }
    }
}
