using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class OpponentSessionLostAnswerMessage : GenericMessage
    {
        public OpponentSessionLostAnswerMessage()
        {
            Type = MessageType.opponentSessionLostAnswer;
        }
    }
}
