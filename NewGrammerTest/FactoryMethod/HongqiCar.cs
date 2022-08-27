using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    internal class HongqiCar : AbstractCar
    {
        public HongqiCar()
        {
            Console.WriteLine("构造了一辆红旗汽车！");
        }
        public override void Run()
        {
            Console.WriteLine("红旗汽车跑起来了！！");
        }

        public override void Startup()
        {
            Console.WriteLine("红旗汽车启动了！！");
        }

        public override void Stop()
        {
            Console.WriteLine("红旗汽车停下来了！！");
        }

        public override void Turn(Direction direction)
        {
            Console.WriteLine($"红旗汽车往{direction}转了！！");
        }
    }

    internal class HongqiCarFactory : CarFactory
    {
        public override AbstractCar CreateCar()
        {
            return new HongqiCar();
        }
    }
}
