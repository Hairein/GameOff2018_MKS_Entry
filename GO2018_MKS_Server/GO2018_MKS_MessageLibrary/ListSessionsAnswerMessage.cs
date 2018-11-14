using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class ListSessionsAnswerMessage : GenericMessage
    {
        public bool Success;
        public string Details;
        public SessionInfo[] Sessions;

        public ListSessionsAnswerMessage()
        {
            Type = MessageType.listSessionsAnswer;

            Success = false;
            Details = string.Empty;

            Sessions = null;
        }

        public ListSessionsAnswerMessage(bool flag, string details, SessionInfo[] newSessions)
        {
            Type = MessageType.listSessionsAnswer;

            Success = flag;
            Details = details;

            Sessions = newSessions;
        }
    }
}
