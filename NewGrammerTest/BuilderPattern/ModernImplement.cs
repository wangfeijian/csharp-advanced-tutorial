using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuilderPattern
{
    public class ModernHouse : House
    {
        public ModernHouse(int doors, int windows, int wall)
        {
            Console.WriteLine($"一个有{doors}条门，{windows}扇窗，{wall}面墙的现代风格房子建造完成。");
        }
    }

    public class ModernHouseBuilder : Builder
    {
        private int _doors, _windows, _wall;

        public ModernHouseBuilder()
        {
            _doors = 0;
            _windows = 0;
            _wall = 0;
        }

        public override void BuildDoor()
        {
            _doors++;
            Console.WriteLine($"构造第{_doors}条现代风格的门。");
        }

        public override void BuildWindows()
        {
            _windows++;
            Console.WriteLine($"构造第{_windows}个现代风格的窗。");
        }

        public override void BuildWall()
        {
            _wall++;
            Console.WriteLine($"构造第{_wall}面现代风格的墙壁。");
        }

        public override void BuildFloor()
        {
            Console.WriteLine("构造现代风格的地板");
        }

        public override void BuildHouseCeiling()
        {
            Console.WriteLine("构造现代风格的天花板");
        }

        public override House GetHouse()
        {
            return new ModernHouse(_doors, _windows, _wall);
        }
    }
}
