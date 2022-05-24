using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace TestDemo
{
    public delegate void ChangeDemo(string input);
    class Program
    {
        static void Main(string[] args)
        {
            //Form formA, formB, formC;
            //var dicTest = new Dictionary<string, Form>
            //{
            //    {"A", new Form() {Text = "A"}}, {"B", new Form() {Text = "B"}}, {"C", new Form() {Text = "C"}}
            //};

            //Console.WriteLine(Calculator.GetCycleArea(3));
            //Console.WriteLine(Calculator.GetCylinder(3,5));
            //Console.WriteLine(Calculator.GetValumn(3,5));
            //Console.WriteLine(Thread.CurrentThread.ToString());
            //var calc = new Calculator();
            //calc.StringChange += Calculator.ChangeName;
            //calc.ShowEvent();
            //int num = Environment.TickCount;
            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(123);
            //}
            //Console.WriteLine(Environment.TickCount-num);

            //dicTest.TryGetValue("A", out formA);
            //dicTest.TryGetValue("B", out formB);
            //dicTest.TryGetValue("C", out formC);

            //formA?.ShowDialog();
            //formB?.Show();
            //formC?.Show();

            var testForm = Activator.CreateInstance(typeof(Form));
            ((Form) testForm).ShowDialog();

            var doc = new XmlDocument();
            doc.Load("test.xml");
            var xnl = doc.SelectNodes("/Project/" + "Cylinder");
            if (xnl != null)
            {
                xnl = xnl.Item(0)?.ChildNodes;
                if (xnl != null)
                    foreach (var xe in xnl)
                    {
                        XmlElement xel = (XmlElement) xe;
                        Console.WriteLine(xel.GetAttribute("名称"));
                        Console.WriteLine(xel.GetAttribute("翻译"));
                        Console.WriteLine(xel.GetAttribute("类型"));
                        Console.WriteLine(xel.GetAttribute("伸出输出"));
                        Console.WriteLine(xel.GetAttribute("缩回输出"));
                        Console.WriteLine(xel.GetAttribute("伸出输入"));
                        Console.WriteLine(xel.GetAttribute("缩回输入"));
                    }
            }
        }
    }

    class Calculator
    {
        public event ChangeDemo StringChange;
        
        public static double GetCycleArea(double radiu)
        {
            return Math.PI * radiu * radiu;
        }

        public static double GetCylinder(double radiu, double height)
        {
            return GetCycleArea(radiu) * height;
        }

        public static double GetValumn(double radiu,double height)
        {
            return GetCylinder(radiu, height) / 3;
        }

        public static void ChangeName(string name)
        {
            Console.WriteLine(name);
        }

        public void ShowEvent()
        {
            StringChange?.Invoke("test");
        }
    }
}
