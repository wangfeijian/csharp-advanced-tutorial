using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

namespace DependencyInjectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.AddJsonFile("jsonconfig.json");
            var config = configuration.Build();
            Console.WriteLine(config["ServiceUrl"]);
            Console.WriteLine(config["Logging:LogLevel:Default"]);

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
