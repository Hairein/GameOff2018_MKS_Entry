using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GO2018_MKS_Server.ServerUtitlity;

namespace GO2018_MKS_Server
{
    public class ActiveSessionInfo
    {
        public ConnectedClientInfo player1 = null;
        public ConnectedClientInfo player2 = null;

        public string MapName = string.Empty;
        public int DurationSeconds = 0;

        public SessionState State;

        public ActiveSessionInfo()
        {
            State = SessionState.waiting;
        }

        public ActiveSessionInfo(ConnectedClientInfo newPlayer1, ConnectedClientInfo newPlayer2, string newMapName, int newDurationSeconds)
        {
            State = SessionState.waiting;

            player1 = newPlayer1;
            player2 = newPlayer2;

            MapName = newMapName;
            DurationSeconds = newDurationSeconds;
        }
    }
}
