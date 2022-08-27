using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    internal class CarTestFramework
    {
        public void BuildTestContext(CarFactory carFactory)
        {
            AbstractCar car1 = carFactory.CreateCar();
            AbstractCar car2 = carFactory.CreateCar();
            AbstractCar car3 = carFactory.CreateCar();
        }

        public void DoTest(CarFactory carFactory)
        {
            AbstractCar car = carFactory.CreateCar();
            car.Startup();
            car.Run();
            car.Turn(Direction.FrontDirection);
            car.Turn(Direction.BackDirection);
            car.Turn(Direction.LeftDirection);
            car.Turn(Direction.RightDirection);
            car.Stop();
        }
    }
}
