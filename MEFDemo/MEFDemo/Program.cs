using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MEFDemo
{
    class Program
    {
        [Import]
        public IBookService Service { get; set; }

        static void Main(string[] args)
        {
            // 1. 首先建立一个接口和一个实现此接口的类，并在实现的接口类上面添加一个特性[Export(typeof(IBookService))]
            // 2. 添加System.ComponentModel.Composition引用
            // 3. 在引用的属性上添加一个特性[Import]

            // 4. 通过下面的语句获取到相应的对象

            Program p = new Program();
            p.Compose();

            Console.WriteLine(p.Service?.GetBookName());

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

    [Export(typeof(IBookService))]
    public class MusicBook : IBookService
    {
        public string BookName { get; set; }
        public string GetBookName()
        {
            return "MusicBook";
        }
    }
}
