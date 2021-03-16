using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDbMgr;

namespace MySqlMgr
{
    public class MySqlHelper:IDbHelper
    {
        public int Id { get; set; } = 1000;
        public string Name { get; set; } = "Test Name";
        public MySqlHelper()
        {
            Console.WriteLine($"{GetType().Name}被构造。");
        }

        public MySqlHelper(int iParam)
        {
            Console.WriteLine($"有参构造函数：{GetType().Name}被构造。传入的值为{iParam}.");
        }

        public MySqlHelper(int iParam, string strParam)
        {
            Console.WriteLine($"多个参数的构造函数：{GetType().Name}被构造。传入的值为{iParam}和{strParam}.");
        }

        public MySqlHelper(string strParam, int iParam)
        {
            Console.WriteLine($"多个参数的构造函数：{GetType().Name}被构造。传入的值为{strParam}和{iParam}.");
        }

        public void Query()
        {
            Console.WriteLine($"{GetType().FullName}被调用。");
        }

        public void ShowOfNonParam()
        {
            Console.WriteLine($"无参数的方法被调用！");
        }

        public void ShowOfOneParam(int iParam)
        {
            Console.WriteLine($"带有一个参数的方法被调用！，参数是{iParam}");
        }

        public void ShowOfTwoParam(int iParam, string strParam)
        {
            Console.WriteLine($"带有两个参数的方法被调用！，参数是{iParam}和{strParam}");
        }

        public void ShowOfTwoParamChange(string strParam, int iParam)
        {
            Console.WriteLine($"带有两个参数的方法被调用！，参数是{strParam}和{iParam}");
        }

        public static void ShowStaticMethon()
        {
            Console.WriteLine($"无参数的静态方法被调用！");
        }
    }

    public class MySqlGeneric<T> : IDbHelper
    {
        public MySqlGeneric(T tParam)
        {
            Console.WriteLine($"泛型类有参构造函数：{GetType().Name}被构造。传入的值为{tParam}.");
        }
        public void Query()
        {
            Console.WriteLine($"{GetType().Name}被调用。");
        }

        public void ShowOfNonParam()
        {
            Console.WriteLine($"无参数的方法被调用！");
        }

        public void ShowOfOneParam(int iParam)
        {
            Console.WriteLine($"带有一个参数的方法被调用！，参数是{iParam}");
        }

        public void ShowGeneric<T>(T tParam)
        {
            Console.WriteLine($"泛型方法被调用！，参数是{tParam}");
        }

        private void ShowPrivateNonParam()
        {
            Console.WriteLine($"私有方法被调用！");
        }
    }
}
