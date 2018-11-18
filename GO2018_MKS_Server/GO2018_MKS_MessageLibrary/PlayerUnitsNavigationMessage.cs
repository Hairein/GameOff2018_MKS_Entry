using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class PlayerUnitsNavigationMessage : GenericMessage
    {
        public UnitNavigationCommand[] NavigationCommands;

        public PlayerUnitsNavigationMessage()
        {
            Type = MessageType.playerUnitsNavigation;
        }

        public PlayerUnitsNavigationMessage(UnitNavigationCommand[] newNavigationCommands)
        {
            Type = MessageType.playerUnitsNavigation;

            NavigationCommands = newNavigationCommands;
        }
    }
}
