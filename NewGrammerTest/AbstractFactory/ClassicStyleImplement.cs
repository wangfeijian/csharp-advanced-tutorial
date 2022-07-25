using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 经典风格的道路实现
    /// </summary>
    public class ClassicRoad : Road
    {
        public ClassicRoad()
        {
            Console.WriteLine("经典风格的道路构造完成！！");
        }
    }

    /// <summary>
    /// 经典风格的建筑实现
    /// </summary>
    public class ClassicBuilding : Building
    {
        public ClassicBuilding()
        {
            Console.WriteLine("经典风格的建筑构造完成！！");
        }
    }

    /// <summary>
    /// 经典风格的隧道实现
    /// </summary>
    public class ClassicTunnel : Tunnel
    {
        public ClassicTunnel()
        {
            Console.WriteLine("经典风格的隧道构造完成！！");
        }
    }

    /// <summary>
    /// 经典风格的丛林实现
    /// </summary>
    public class ClassicJungle : Jungle
    {
        public ClassicJungle()
        {
            Console.WriteLine("经典风格的丛林构造完成！！");
        }
    }

    /// <summary>
    /// 经典风格的工厂实现
    /// </summary>
    public class ClassicFactory : FacilitiesFactory
    {
        public override Building CreateBuilding()
        {
            return new ClassicBuilding();
        }

        public override Jungle CreateJungle()
        {
            return new ClassicJungle();
        }

        public override Road CreateRoad()
        {
            return new ClassicRoad();
        }

        public override Tunnel CreateTunnel()
        {
            return new ClassicTunnel();
        }
    }
}
