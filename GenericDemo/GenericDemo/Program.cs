using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDemo
{
    class Program
    {
        static void Main()
        {
            int iValue = 100;
            string strValue = "test";
            DateTime datetimeValue = DateTime.Now;
            object oValue = 33.54;

            // 普通方法实现
            CommonMethod(iValue, strValue, datetimeValue, oValue);
            Console.WriteLine();

            // 泛型方法实现
            GenericMethod(iValue, strValue, datetimeValue, oValue);
            Console.WriteLine();

            // 性能比较
            RunTimeTest();
            Console.WriteLine();

            //泛型类输出
            GenericClass();
        }

        static void CommonMethod(int iValue, string strValue, DateTime datetimeValue, object oValue)
        {
            Console.WriteLine(".net 1.0中实现一个方法调用多个参数");

            // 在.net 1.0中，如果需要显示不能类型的参数的值和类型，需要编写多个相同的代码来完成
            Common.ShowInt(iValue);
            Common.ShowString(strValue);
            Common.ShowDatetime(datetimeValue);

            // 也可以使用object来实现，但是最终在使用的时候需要进行类型转换
            // 有可能产生装箱拆箱的操作，会造成性能的损失
            Common.ShowObject(oValue);
        }

        static void GenericMethod(int iValue, string strValue, DateTime datetimeValue, object oValue)
        {
            Console.WriteLine(".net 2.0中引入了泛型，就可以利用一个方法实现");

            // Generic.Show<in>(iValue);
            // 上一行中的尖括号可以省略，因为编译器会自动推算是什么类型。
            Generic.Show(iValue);
            Generic.Show(strValue);
            Generic.Show(datetimeValue);
            Generic.Show(oValue);
        }

        static void RunTimeTest()
        {
            Console.WriteLine("通过3种方法来验证传入参数的运行时间，通过.net自带的计时器");

            // 在一个方法中调用，循环传参一亿次计算时间
            Montior.ShowTime();
        }

        static void GenericClass()
        {
            var generic = new GenericClass<People,People,People>();
            var people = new People{Id = 100, Name = "wang"};
            generic.ShowTType(people);
            generic.Show(people);
        }
    }
}
