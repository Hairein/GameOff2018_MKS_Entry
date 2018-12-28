using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class SessionChatMessage : GenericMessage
    {
        public string Message;

        public SessionChatMessage()
        {
            Type = MessageType.sessionChat;

            Message = string.Empty;
        }

        public SessionChatMessage(string message)
        {
            Type = MessageType.sessionChat;

            Message = message;
        }
    }
}
