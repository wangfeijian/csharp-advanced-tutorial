using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory
{
    /// <summary>
    /// 游戏管理类
    /// </summary>
    public class GameManager
    {
        FacilitiesFactory _facilitiesFactory;
        Road? _road;
        Building? _building;
        Jungle? _jungle;
        Tunnel? _tunnel;

        public GameManager(FacilitiesFactory facilitiesFactory)
        {
            _facilitiesFactory = facilitiesFactory;
        }

        public void BulidGameFacility()
        {
            _road = _facilitiesFactory.CreateRoad();
            _building = _facilitiesFactory.CreateBuilding();
            _jungle = _facilitiesFactory.CreateJungle();
            _tunnel = _facilitiesFactory.CreateTunnel();
            Console.WriteLine("对象构造完成");
        }
    }
}
