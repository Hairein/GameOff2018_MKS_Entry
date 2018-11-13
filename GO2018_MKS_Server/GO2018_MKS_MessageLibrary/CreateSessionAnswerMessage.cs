using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class CreateSessionAnswerMessage : GenericMessage
    {
        public bool Success;
        public string Details;

        public CreateSessionAnswerMessage()
        {
            Type = MessageType.createSessionAnswer;

            Success = false;
            Details = string.Empty;
        }

        public CreateSessionAnswerMessage(bool flag, string details)
        {
            Type = MessageType.createSessionAnswer;

            Success = flag;
            Details = details;
        }
    }
}
