using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MEFDemoForName
{
    class Program
    {
        //[Import("MusicBook")]
        //public IBookService Service { get; set; }

        [ImportMany("MusicBook")]
        public IEnumerable<IBookService> Services { get; set; }

        static void Main(string[] args)
        {
            // 1. 首先建立一个接口和一个实现此接口的类，并在实现的接口类上面添加一个特性[Export("MusicBook",typeof(IBookService))]
            // 2. 添加System.ComponentModel.Composition引用
            // 3. 在引用的属性上添加一个特性[Import("MusicBook")]

            // 4. 通过下面的语句获取到相应的对象
            // 这个demo和前面一个唯一不一样的地方就是加了一个名字
            // 添加多几个类使用IEnumerable来实现

            Program p = new Program();
            p.Compose();

            if (p.Services != null)
            {
                foreach (var s in p.Services)
                {
                    Console.WriteLine(s.GetBookName());
                }
            }

            //Console.WriteLine(p.Service?.GetBookName());

            Console.ReadKey();
        }

        private void Compose()
        {
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
    }

    public interface IBookService
    {
        string BookName { get; set; }
        string GetBookName();
    }

    [Export("MusicBook",typeof(IBookService))]
    public class MusicBook : IBookService
    {
        public string BookName { get; set; }
        public string GetBookName()
        {
            return "MusicBook";
        }
    }

    //[Export("MusicBook", typeof(IBookService))]
    [Export("MathBook", typeof(IBookService))]
    public class MathBook : IBookService
    {
        public string BookName { get; set; }

        public string GetBookName()
        {
            return "MathBook";
        }
    }

    //[Export("MusicBook", typeof(IBookService))]
    [Export("HistoryBook", typeof(IBookService))]
    public class HistoryBook : IBookService
    {
        public string BookName { get; set; }

        public string GetBookName()
        {
            return "HistoryBook";
        }
    }
}
