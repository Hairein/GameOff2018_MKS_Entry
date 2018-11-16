using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class JoinSessionMessage : GenericMessage
    {
        public SessionInfo session;

        public JoinSessionMessage()
        {
            Type = MessageType.joinSession;

            session = null;
        }

        public JoinSessionMessage(SessionInfo newSession)
        {
            Type = MessageType.joinSession;

            session = newSession;
        }
    }
}
