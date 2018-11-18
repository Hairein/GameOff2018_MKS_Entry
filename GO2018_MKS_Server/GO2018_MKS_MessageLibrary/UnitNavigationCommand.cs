using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class UnitNavigationCommand
    {
        public string[] UnitNames = null;
        public WorldCoordinate NavigationPoint = new WorldCoordinate();

        public UnitNavigationCommand()
        {
        }

        public UnitNavigationCommand(string[] newNames, WorldCoordinate newNavigationPoint)
        {
            UnitNames = newNames;
            NavigationPoint = newNavigationPoint;
        }
    }
}
