using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class SessionChatAnswerMessage : GenericMessage
    {
        public string Message;

        public SessionChatAnswerMessage()
        {
            Type = MessageType.sessionChatAnswer;

            Message = string.Empty;
        }

        public SessionChatAnswerMessage(string message)
        {
            Type = MessageType.sessionChatAnswer;

            Message = message;
        }
    }
}
