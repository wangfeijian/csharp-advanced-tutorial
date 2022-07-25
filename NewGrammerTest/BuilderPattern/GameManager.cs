using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuilderPattern
{
    public class GameManager
    {
        public static House CreateHouse(Builder builder)
        {
            builder.BuildDoor();
            builder.BuildDoor();

            builder.BuildWindows();
            builder.BuildWindows();

            builder.BuildWall();
            builder.BuildWall();
            builder.BuildWall();
            builder.BuildWall();

            builder.BuildFloor();
            builder.BuildHouseCeiling();

            return builder.GetHouse();
        }
    }
}
