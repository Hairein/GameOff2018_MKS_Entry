using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class EndSessionAnswerMessage : GenericMessage
    {
        public EndSessionAnswerMessage()
        {
            Type = MessageType.endSessionAnswer;
        }
    }
}
