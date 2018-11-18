using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class PlayerUnitsUpdateMessage : GenericMessage
    {
        public UnitResourceState[] UnitResourceStates = null;

        public PlayerUnitsUpdateMessage()
        {
            Type = MessageType.playerUnitsUpdate;
        }

        public PlayerUnitsUpdateMessage(UnitResourceState[] newUnitResourceStates)
        {
            Type = MessageType.playerUnitsUpdate;

            UnitResourceStates = newUnitResourceStates;
        }
    }
}
