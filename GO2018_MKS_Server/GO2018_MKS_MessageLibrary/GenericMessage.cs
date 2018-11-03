using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class GenericMessage
    {
        public MessageType Type;

        public GenericMessage()
        {
            Type = MessageType.generic;
        }
    }
}
