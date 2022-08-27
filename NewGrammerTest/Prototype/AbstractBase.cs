using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    [Serializable]
    public class Gan
    { 

    }
    

    [Serializable]
    public abstract class NormalActorBase : ICloneable
    {
        public int ID { get; set; }

        public Gan NormalGan { get; set; }

        public abstract object Clone();
    }

    public abstract class FlyActorBase : ICloneable
    {
        public int ID { get; set; }

        public Gan FlyGan { get; set; }

        public abstract object Clone();
    }

    public abstract class WaterActorBase : ICloneable
    {
        public int ID { get; set; }

        public abstract object Clone();
    }
}
