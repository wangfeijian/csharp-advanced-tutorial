using System;
using System.Reflection;
using IDbMgr;
using MySqlMgr;
using SqlServiceMgr;
using System.Configuration;
using System.Diagnostics;

namespace ReflectionDemo
{

    class Program
    {
        static readonly System.Collections.Specialized.NameValueCollection ConfigStr = ConfigurationManager.AppSettings;
        static readonly string ConfigKey = ConfigStr["DllKey"];
        static readonly string ConfigDllName = ConfigKey.Split(',')[0];
        static readonly string ConfigClassName = ConfigKey.Split(',')[1];


        static void Main()
        {
            // 普通的多态调用
            CommonCall();

            // 反射调用无参构造函数生成实例
            ReflectionCallNonParameterOfConstructor();

            // 反射调用有参构造函数生成实例
            ReflectionCallParameterOfConstructor();

            // 反射调用泛型类有参构造函数生成实例
            ReflectionCallGenericClass();

            // 反射调用普通类中的方法
            ReflectionCallCommonMethod();

            // 反射调用泛型类中的方法
            ReflectionCallGenericMethod();

            // 反射访问实例中的属性
            ReflectionGetProperty();

            // 利用反射动态加载库文件实现软件的可配置可扩展
            ReflectionDynamicLoadConfig();

            // 测试反射和普通创建对象的性能
            TestPerformanceForReflectionAndCommon();
        }

        static void CommonCall()
        {
            IDbHelper iDbHelper = new MySqlHelper();
            iDbHelper.Query();
            Console.WriteLine("*************************我是分割线，普通调用***********************\n");

            iDbHelper = new SqlDbHelper();
            iDbHelper.Query();
            Console.WriteLine("*************************我是分割线，普通调用***********************\n");
        }

        static void ReflectionCallNonParameterOfConstructor()
        {
            // 加载库文件的相对路径，需要将扩展名加上
            //Assembly testAssembly = Assembly.LoadFrom("MySqlMgr.dll");
            //Type tesType = testAssembly.GetType("MySqlMgr.MySqlHelper");

            // 通过方法直接构造
            var o = Activator.CreateInstance("MySqlMgr", "MySqlMgr.MySqlHelper", null);
            IDbHelper i = (IDbHelper)o.Unwrap();
            i.Query();
            Console.WriteLine("*************************我是分割线，反射调用无参构造函数***********************\n");

            // 直接从程序的运行目录下查找Dll文件，不需要加扩展名
            Assembly mySqlAssembly = Assembly.Load("MySqlMgr");
            Type mySqlType = mySqlAssembly.GetType("MySqlMgr.MySqlHelper");
            object oDb = Activator.CreateInstance(mySqlType);
            IDbHelper iDbHelper = (IDbHelper)oDb;
            iDbHelper.Query();
            Console.WriteLine("*************************我是分割线，反射调用无参构造函数***********************\n");

            // 利用文件的绝对路径加载Dll文件
            Assembly sqlServiceAssembly = Assembly.LoadFile(@"D:\Study\dotNet\ReflectionDemo\ReflectionDemo\bin\Debug\SqlServiceMgr.dll");
            Type sqlServiceType = sqlServiceAssembly.GetType("SqlServiceMgr.SqlDbHelper");
            oDb = Activator.CreateInstance(sqlServiceType);
            iDbHelper = (IDbHelper)oDb;
            iDbHelper.Query();
            Console.WriteLine("*************************我是分割线，反射调用无参构造函数***********************\n");
        }

        static void ReflectionCallParameterOfConstructor()
        {
            // 直接从程序的运行目录下查找Dll文件，不需要加扩展名
            Assembly mySqlAssembly = Assembly.Load("MySqlMgr");
            Type mySqlType = mySqlAssembly.GetType("MySqlMgr.MySqlHelper");

            // 通过传入参数，直接调用有一个参数的构造函数
            object oDb = Activator.CreateInstance(mySqlType, 123);
            IDbHelper iDbHelper = (IDbHelper)oDb;
            iDbHelper.Query();
            Console.WriteLine("*************************我是分割线，反射调用一个参数的构造函数***********************\n");

            // 通过传入参数，直接调用有两个参数的构造函数
            oDb = Activator.CreateInstance(mySqlType, 100, "test");
            iDbHelper = (IDbHelper)oDb;
            iDbHelper.Query();
            Console.WriteLine("*************************我是分割线，反射调用两个参数的构造函数***********************\n");

            // 通过传入参数，直接调用有两个参数的构造函数
            oDb = Activator.CreateInstance(mySqlType, "test", 100);
            iDbHelper = (IDbHelper)oDb;
            iDbHelper.Query();
            Console.WriteLine("*************************我是分割线，反射调用两个参数的构造函数***********************\n");
        }

        static void ReflectionCallGenericClass()
        {
            // 直接从程序的运行目录下查找Dll文件，不需要加扩展名
            Assembly mySqlAssembly = Assembly.Load("MySqlMgr");
            Type mySqlType = mySqlAssembly.GetType("MySqlMgr.MySqlGeneric`1");
            Type[] paramTypes = { typeof(int) };
            Type instanceType = mySqlType.MakeGenericType(paramTypes);

            // 通过传入参数，直接调用有一个参数的构造函数传入int
            object oDb = Activator.CreateInstance(instanceType, 123);
            IDbHelper iDbHelper = (IDbHelper)oDb;
            iDbHelper.Query();
            Console.WriteLine("*************************我是分割线，反射调用泛型类一个参数的构造函数***********************\n");

            // 通过传入参数，直接调用有一个参数的构造函数传入string
            Type[] paramStrTypes = { typeof(string) };
            instanceType = mySqlType.MakeGenericType(paramStrTypes);
            object oDbStr = Activator.CreateInstance(instanceType, "Test");
            IDbHelper iDbHelperStr = (IDbHelper)oDbStr;
            iDbHelperStr.Query();
            Console.WriteLine("*************************我是分割线，反射调用泛型类一个参数的构造函数***********************\n");
        }

        static void ReflectionCallCommonMethod()
        {
            // 直接从程序的运行目录下查找Dll文件，不需要加扩展名
            Assembly mySqlAssembly = Assembly.Load("MySqlMgr");
            Type mySqlType = mySqlAssembly.GetType("MySqlMgr.MySqlHelper");

            // 调用无参数构造函数的无参数方法
            string str = "*************************我是分割线，反射调用无参构造函数中的无参方法***********************\n";
            CallMethod(mySqlType, "ShowOfNonParam", str, null, null);

            // 调用有参数构造函数的无参数方法
            str = "*************************我是分割线，反射调用有参构造函数中的无参方法***********************\n";
            CallMethod(mySqlType, "ShowOfNonParam", str, new object[] { 123 }, null);

            // 调用无参数构造函数中的一个参数方法
            str = "*************************我是分割线，反射调用无参构造函数中的有参方法***********************\n";
            CallMethod(mySqlType, "ShowOfOneParam", str, null, new object[] { 123 });

            // 调用有参数构造函数的的一个参数方法
            str = "*************************我是分割线，反射调用有参构造函数中的有参方法***********************\n";
            CallMethod(mySqlType, "ShowOfOneParam", str, new object[] { 123 }, new object[] { 123 });

            // 调用无参数构造函数中的多个参数方法
            str = "*************************我是分割线，反射调用无参构造函数中的有参方法***********************\n";
            CallMethod(mySqlType, "ShowOfTwoParam", str, null, new object[] { 123, "test" });
            CallMethod(mySqlType, "ShowOfTwoParamChange", str, null, new object[] { "test", 123 });

            // 调用有参数构造函数的的多个参数方法
            str = "*************************我是分割线，反射调用有参构造函数中的有参方法***********************\n";
            CallMethod(mySqlType, "ShowOfTwoParam", str, new object[] { 123 }, new object[] { 123, "test" });
            CallMethod(mySqlType, "ShowOfTwoParamChange", str, new object[] { 123 }, new object[] { "test", 123 });

            // 调用无参构造函数的静态无参方法
            str = "*************************我是分割线，反射调用无参构造函数中的静态方法***********************\n";
            CallMethod(mySqlType, "ShowStaticMethon", str, null, null);
        }

        /// <summary>
        /// 调用实例中的方法
        /// </summary>
        /// <param name="mySqlType">获取的实例类型</param>
        /// <param name="strMethodName">方法名称</param>
        /// <param name="displayInfo">显示信息</param>
        /// <param name="objParam">类构造参数</param>
        /// <param name="objMethondParam">方法运行参数</param>
        static void CallMethod(Type mySqlType, string strMethodName, string displayInfo, object[] objParam, object[] objMethondParam)
        {
            var oDb = Activator.CreateInstance(mySqlType, objParam);
            var method = mySqlType.GetMethod(strMethodName);
            method?.Invoke(oDb, objMethondParam);
            Console.WriteLine(displayInfo);
        }

        static void ReflectionCallGenericMethod()
        {
            // 直接从程序的运行目录下查找Dll文件，不需要加扩展名
            Assembly mySqlAssembly = Assembly.Load("MySqlMgr");
            Type mySqlType = mySqlAssembly.GetType("MySqlMgr.MySqlGeneric`1");

            Type[] paramTypes = { typeof(int) };
            Type instanceType = mySqlType.MakeGenericType(paramTypes);

            // 通过传入参数，直接调用有一个参数的构造函数传入int
            object oDb = Activator.CreateInstance(instanceType, 123);

            // 调用普通方法
            // 此处注意，如果是调用泛型类的话，获取方法需要使用MakeGenericType后的类型
            var method = instanceType.GetMethod("ShowOfNonParam");
            method?.Invoke(oDb, null);
            Console.WriteLine("*************************我是分割线，反射调用泛型类中的普通方法***********************\n");

            // 调用有参方法
            method = instanceType.GetMethod("ShowOfOneParam");
            method?.Invoke(oDb, new object[] { 123 });
            Console.WriteLine("*************************我是分割线，反射调用泛型类中的有参方法***********************\n");

            // 调用泛型方法
            method = instanceType.GetMethod("ShowGeneric");
            method = method?.MakeGenericMethod(typeof(int));
            method?.Invoke(oDb, new object[] { 123 });
            Console.WriteLine("*************************我是分割线，反射调用泛型类中的泛型方法***********************\n");

            // 调用私有方法
            method = instanceType.GetMethod("ShowPrivateNonParam", BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(oDb, null);
            Console.WriteLine("*************************我是分割线，反射调用泛型类中的私有方法***********************\n");
        }

        static void ReflectionGetProperty()
        {
            // 直接从程序的运行目录下查找Dll文件，不需要加扩展名
            Assembly mySqlAssembly = Assembly.Load("MySqlMgr");
            Type mySqlType = mySqlAssembly.GetType("MySqlMgr.MySqlHelper");

            var oDb = Activator.CreateInstance(mySqlType);

            // 获取属性
            var property = mySqlType.GetProperty("Id");
            Console.WriteLine($"{property?.Name}:{property?.GetValue(oDb)}");
            Console.WriteLine("*************************我是分割线，反射调用泛型类中的属性***********************\n");

            property = mySqlType.GetProperty("Name");
            Console.WriteLine($"{property?.Name}:{property?.GetValue(oDb)}");
            Console.WriteLine("*************************我是分割线，反射调用泛型类中的属性***********************\n");

            var properties = mySqlType.GetProperties();
            foreach (var item in properties)
            {
                property = mySqlType.GetProperty(item.Name);
                Console.WriteLine($"{item.Name}:{property?.GetValue(oDb)}");
            }
            Console.WriteLine("*************************我是分割线，反射遍历类型中所有的属性***********************\n");
        }

        static void ReflectionDynamicLoadConfig()
        {
            // 通过方法直接构造
            var o = Activator.CreateInstance(ConfigDllName, ConfigClassName, null);
            IDbHelper i = (IDbHelper)o.Unwrap();
            i.Query();
            Console.WriteLine("*************************我是分割线，反射利用配置文件调用Dll***********************\n");
        }

        static void TestPerformanceForReflectionAndCommon()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                IDbHelper iDbHelper = new MySqlHelper();
            }
            watch.Stop();

            var commonTimeSpan = watch.Elapsed.TotalSeconds;

            watch.Reset();
            watch.Start();

            // 将加载Dll文件放到循环外面来
            Assembly mySqlAssembly = Assembly.Load("MySqlMgr");
            Type mySqlType = mySqlAssembly.GetType("MySqlMgr.MySqlHelper");
            for (int i = 0; i < 1000000; i++)
            {
                //Assembly mySqlAssembly = Assembly.Load("MySqlMgr");
                //Type mySqlType = mySqlAssembly.GetType("MySqlMgr.MySqlHelper");
                object oDb = Activator.CreateInstance(mySqlType);
            }
            watch.Stop();

            var reflectionTimeSpan = watch.Elapsed.TotalSeconds;

            Console.WriteLine($"Common timespan:{commonTimeSpan}");
            Console.WriteLine($"Reflection timespan:{reflectionTimeSpan}");
            Console.WriteLine("*************************我是分割线，测试反射和直接调用的性能***********************\n");
        }
    }
}
