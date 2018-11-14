using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

namespace GO2018_MKS_MessageLibrary
{
    public class SessionInfo
    {
        public string MapName;
        public string OpponentHandle;
        public SessionTeam SuggestedTeam;
        public int DurationSeconds;

        public SessionInfo()
        {
            MapName = string.Empty;
            OpponentHandle = string.Empty;
            SuggestedTeam = SessionTeam.blue;
            DurationSeconds = 0;
        }

        public SessionInfo(string newMapName, string newOpponentHandle, SessionTeam newSuggestedTeam, int newDurationSeconds)
        {
            MapName = newMapName;
            OpponentHandle = newOpponentHandle;
            SuggestedTeam = newSuggestedTeam;
            DurationSeconds = newDurationSeconds;
        }
    }
}
