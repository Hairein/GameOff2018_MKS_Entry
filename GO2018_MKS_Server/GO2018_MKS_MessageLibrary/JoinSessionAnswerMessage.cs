using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class JoinSessionAnswerMessage : GenericMessage
    {
        public bool Success;
        public string Details;

        public JoinSessionAnswerMessage()
        {
            Type = MessageType.joinSessionAnswer;

            Success = false;
            Details = string.Empty;
        }

        public JoinSessionAnswerMessage(bool flag, string details)
        {
            Type = MessageType.joinSessionAnswer;

            Success = flag;
            Details = details;
        }
    }
}
