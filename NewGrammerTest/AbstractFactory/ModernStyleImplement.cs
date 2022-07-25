using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 现代风格的道路实现
    /// </summary>
    public class ModernRoad : Road
    {
        public ModernRoad()
        {
            Console.WriteLine("现代风格的道路构造完成！！");
        }
    }

    /// <summary>
    /// 现代风格的建筑实现
    /// </summary>
    public class ModernBuilding : Building
    {
        public ModernBuilding()
        {
            Console.WriteLine("现代风格的建筑构造完成！！");
        }
    }

    /// <summary>
    /// 现代风格的隧道实现
    /// </summary>
    public class ModernTunnel : Tunnel
    {
        public ModernTunnel()
        {
            Console.WriteLine("现代风格的隧道构造完成！！");
        }
    }

    /// <summary>
    /// 现代风格的丛林实现
    /// </summary>
    public class ModernJungle : Jungle
    {
        public ModernJungle()
        {
            Console.WriteLine("现代风格的丛林构造完成！！");
        }
    }

    /// <summary>
    /// 现代风格的工厂实现
    /// </summary>
    public class ModernFactory : FacilitiesFactory
    {
        public override Building CreateBuilding()
        {
            return new ModernBuilding();
        }

        public override Jungle CreateJungle()
        {
            return new ModernJungle();
        }

        public override Road CreateRoad()
        {
            return new ModernRoad();
        }

        public override Tunnel CreateTunnel()
        {
            return new ModernTunnel();
        }
    }
}
