using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.IO;
using System.Threading;

namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Calculator.GetCycleArea(3));
            Console.WriteLine(Calculator.GetCylinder(3,5));
            Console.WriteLine(Calculator.GetValumn(3,5));
            Console.WriteLine(Thread.CurrentThread.ToString());
        }
    }

    class Calculator
    {
        
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
    }
}
