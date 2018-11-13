using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

namespace GO2018_MKS_MessageLibrary
{
    public class CreateSessionMessage : GenericMessage
    {
        public string MapName;
        public SessionTeam OwnTeam;
        public int SessionSeconds;

        public CreateSessionMessage()
        {
            Type = MessageType.createSession;

            MapName = string.Empty;
            OwnTeam = SessionTeam.blue;
            SessionSeconds = 0;
        }

        public CreateSessionMessage(string newMapName, SessionTeam newTeam, int newSessionSeconds)
        {
            Type = MessageType.createSession;

            MapName = newMapName;
            OwnTeam = newTeam;
            SessionSeconds = newSessionSeconds;
        }
    }
}
