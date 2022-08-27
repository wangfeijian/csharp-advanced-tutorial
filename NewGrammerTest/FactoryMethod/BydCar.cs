using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    internal class BydCar : AbstractCar
    {
        public BydCar()
        {
            Console.WriteLine("构造了一辆比亚迪汽车！");
        }
        public override void Run()
        {
            Console.WriteLine("比亚迪汽车跑起来了！！");
        }

        public override void Startup()
        {
            Console.WriteLine("比亚迪汽车启动了！！");
        }

        public override void Stop()
        {
            Console.WriteLine("比亚迪汽车停下来了！！");
        }

        public override void Turn(Direction direction)
        {
            Console.WriteLine($"比亚迪汽车往{direction}转了！！");
        }
    }

    internal class BydCarFactory : CarFactory
    {
        public override AbstractCar CreateCar()
        {
            return new BydCar();
        }
    }
}
