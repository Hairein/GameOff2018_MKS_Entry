using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class MinesUpdateMessage : GenericMessage
    {
        public MineResourceState[] MineResourceStates = null;

        public MinesUpdateMessage()
        {
            Type = MessageType.mineResourcesUpdate;
        }

        public MinesUpdateMessage(MineResourceState[] newMineResourceStates)
        {
            Type = MessageType.mineResourcesUpdate;

            MineResourceStates = newMineResourceStates;
        }
    }
}
