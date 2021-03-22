using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionDemo
{
    public interface ICar
    {
        string Name { get; set; }
        void Run();
    }

    public class Car : ICar
    {
        public string Name { get; set; }

        public Car(string name)
        {
            Name = name;
        }
        public void Run()
        {
            Console.WriteLine($"{Name} Run Fast");
        }
    }

    public class Bus : ICar
    {
        public Bus(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public void Run()
        {
            Console.WriteLine($"{Name} Run slowly");
        }
    }

    public interface IDriver
    {
        void Driver();
    }

    public class OldDriver : IDriver
    {
        private ICar car;
        public OldDriver(ICar car)
        {
            this.car = car;
        }
        public void Driver()
        {
            Console.WriteLine($"老司机开{car.Name}");
            car.Run();
            Console.WriteLine("老司机车开的很好");
        }
    }

    public class WomanDriver : IDriver
    {
        private ICar car;
        public WomanDriver(ICar car)
        {
            this.car = car;
        }
        public void Driver()
        {
            Console.WriteLine($"女司机开{car.Name}");
            car.Run();
            Console.WriteLine("女司机开车技术不好");
        }
    }
}
