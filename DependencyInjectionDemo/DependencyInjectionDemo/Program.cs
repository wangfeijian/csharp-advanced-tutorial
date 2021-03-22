using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // 准备一个服务容器
            IServiceCollection collection = new ServiceCollection();

            // 向服务容器中添加所需要的服务
            collection.AddSingleton("mini");
            collection.AddSingleton<ICar, Car>();
            collection.AddSingleton<IDriver, OldDriver>();

            //collection.AddScoped<ICar, Car>(x => new Car("mini"));
            //collection.AddScoped<IDriver, WomanDriver>();

            // 注册服务
            ServiceProvider service = collection.BuildServiceProvider();

            // 获取服务
            var driver = service.GetRequiredService<IDriver>();
            driver.Driver();

        }
    }
}
