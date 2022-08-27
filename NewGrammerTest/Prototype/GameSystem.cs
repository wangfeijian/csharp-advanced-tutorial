using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    public class GameSystem
    {
        public void Run(NormalActorBase normal, FlyActorBase fly, WaterActorBase water)
        {
            NormalActorBase normal1 = normal.Clone() as NormalActorBase;
            NormalActorBase normal2 = normal.Clone() as NormalActorBase;
            Console.WriteLine($"NormalActor两个对象引用是否一样：{ReferenceEquals(normal1, normal2)}");
            Console.WriteLine($"NormalActor两个对象中的引用对象引用是否一样：{ReferenceEquals(normal1.NormalGan, normal2.NormalGan)}");

            FlyActorBase fly1 = fly.Clone() as FlyActorBase;
            FlyActorBase fly2 = fly.Clone() as FlyActorBase;
            Console.WriteLine($"FlyActor两个对象引用是否一样：{ReferenceEquals(fly1, fly2)}");
            Console.WriteLine($"FlyActor两个对象中的引用对象引用是否一样：{ReferenceEquals(fly1.FlyGan, fly2.FlyGan)}");

            WaterActorBase water1 = water.Clone() as WaterActorBase;
            WaterActorBase water2 = water.Clone() as WaterActorBase;
        }
    }
}
