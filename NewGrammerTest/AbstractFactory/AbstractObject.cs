using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 抽象路，需要具体风格实现
    /// </summary>
    public abstract class Road { }

    /// <summary>
    /// 抽象建筑，需要具体风格实现
    /// </summary>
    public abstract class Building { }

    /// <summary>
    /// 抽象丛林，需要具体风格实现
    /// </summary>
    public abstract class Jungle { }

    /// <summary>
    /// 抽象隧道，需要具体风格实现
    /// </summary>
    public abstract class Tunnel { }

    /// <summary>
    /// 抽象工厂，需要具体的工厂实现
    /// </summary>
    public abstract class FacilitiesFactory
    {
        /// <summary>
        /// 根据具体工厂返回具体的道路
        /// </summary>
        /// <returns></returns>
        public abstract Road CreateRoad();

        /// <summary>
        /// 根据具体工厂返回具体的建筑
        /// </summary>
        /// <returns></returns>
        public abstract Building CreateBuilding();

        /// <summary>
        /// 根据具体工厂返回具体的隧道
        /// </summary>
        /// <returns></returns>
        public abstract Tunnel CreateTunnel();

        /// <summary>
        /// 根据具体工厂返回具体的丛林
        /// </summary>
        /// <returns></returns>
        public abstract Jungle CreateJungle();
    }
}
