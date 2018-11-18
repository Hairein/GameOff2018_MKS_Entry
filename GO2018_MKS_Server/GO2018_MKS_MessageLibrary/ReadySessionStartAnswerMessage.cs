using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class ReadySessionStartAnswerMessage : GenericMessage
    {
        public bool Success;
        public string Details;

        public ReadySessionStartAnswerMessage()
        {
            Type = MessageType.readySessionStartAnswer;

            Success = false;
            Details = string.Empty;
        }

        public ReadySessionStartAnswerMessage(bool flag, string details)
        {
            Type = MessageType.readySessionStartAnswer;

            Success = flag;
            Details = details;
        }
    }
}
