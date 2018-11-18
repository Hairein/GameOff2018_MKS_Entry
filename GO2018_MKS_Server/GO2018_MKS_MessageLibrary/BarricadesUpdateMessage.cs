using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class BarricadesUpdateMessage : GenericMessage
    {
        public BarricadeResourceState[] Barricades = null;

        public BarricadesUpdateMessage()
        {
            Type = MessageType.barricadesUpdate;
        }

        public BarricadesUpdateMessage(BarricadeResourceState[] newBarricades)
        {
            Type = MessageType.barricadesUpdate;

            Barricades = newBarricades;
        }
    }
}
