using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLibrary;

namespace CsScriptsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            dynamic script = CSScript.LoadCode(
                           @"using System.Windows.Forms;
                             public class Script
                             {
                                 public void SayHello(string greeting)
                                 {
                                     MessageBox.Show(""Greeting: "" + greeting);
                                 }
                             }")
                             .CreateObject("*");
            script.SayHello("Hello World!");

            var product = CSScript.CreateFunc<int>(@"int Product(int a, int b)
                                         {
                                             return a * b;
                                         }");
            int result = product(3, 4);
            Console.WriteLine(result);

            var SayHello = CSScript.LoadMethod(
                        @"using System.Windows.Forms;
                          public static void SayHello(string greeting)
                          {
                              MessageBoxSayHello(greeting);
                              ConsoleSayHello(greeting);
                          }
                          static void MessageBoxSayHello(string greeting)
                          {
                              MessageBox.Show(greeting);
                          }
                          static void ConsoleSayHello(string greeting)
                          {
                              Console.WriteLine(greeting);
                          }")
                         .GetStaticMethod("*.SayHello", "");
            SayHello("Hello again!");

            dynamic fileScript = CSScript.Load("helloworld.cs").CreateObject("*");
            fileScript.SayHello();

            Console.ReadKey();
        }
    }
}
