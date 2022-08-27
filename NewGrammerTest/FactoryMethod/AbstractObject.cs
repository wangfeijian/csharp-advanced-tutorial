using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    enum Direction
    {
        FrontDirection,
        BackDirection,
        LeftDirection,
        RightDirection
    }

    internal abstract class AbstractCar
    {
        public abstract void Startup();
        public abstract void Run();
        public abstract void Stop();
        public abstract void Turn(Direction direction);
    }

    internal abstract class CarFactory
    {
        public abstract AbstractCar CreateCar();
    }
}
