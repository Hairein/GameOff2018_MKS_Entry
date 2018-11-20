using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class SessionUpdateAnswerMessage : GenericMessage
    {
        public float SessionTimeLeft;

        public UnitNavigationCommand[] Player1UnitNavigationCommands = null;
        public UnitNavigationCommand[] Player2UnitNavigationCommands = null;

        public UnitResourceState[] Player1UnitResourceStates = null;
        public UnitResourceState[] Player2UnitResourceStates = null;

        public MineResourceState[] MineResourceStates = null;

        public BarricadeResourceState[] BarricadeResourceStates = null;

        public int Player1Score = 0;
        public int Player2Score = 0;

        public SessionUpdateAnswerMessage()
        {
            Type = MessageType.sessionUpdateAnswer;
        }

        public SessionUpdateAnswerMessage(CollectSessionUpdateAnswers collected)
        {
            Type = MessageType.sessionUpdateAnswer;

            SessionTimeLeft = collected.SessionTimeLeft;

            Player1UnitNavigationCommands = collected.Player1UnitNavigationCommands.ToArray();
            Player2UnitNavigationCommands = collected.Player2UnitNavigationCommands.ToArray();

            Player1UnitResourceStates = collected.Player1UnitResourceStates.ToArray();
            Player2UnitResourceStates = collected.Player2UnitResourceStates.ToArray();

            MineResourceStates = collected.MineResourceStates.ToArray();

            BarricadeResourceStates = collected.BarricadeResourceStates.ToArray();

            ConvertScores(collected);
        }

        public void Convert(CollectSessionUpdateAnswers collected)
        {
            Type = MessageType.sessionUpdateAnswer;

            SessionTimeLeft = collected.SessionTimeLeft;

            Player1UnitNavigationCommands = collected.Player1UnitNavigationCommands.ToArray();
            Player2UnitNavigationCommands = collected.Player2UnitNavigationCommands.ToArray();

            Player1UnitResourceStates = collected.Player1UnitResourceStates.ToArray();
            Player2UnitResourceStates = collected.Player2UnitResourceStates.ToArray();

            MineResourceStates = collected.MineResourceStates.ToArray();

            BarricadeResourceStates = collected.BarricadeResourceStates.ToArray();

            ConvertScores(collected);
        }

        private void ConvertScores(CollectSessionUpdateAnswers collected)
        {
            Player1Score = (int)collected.Player1Score;
            Player2Score = (int)collected.Player2Score;
        }
    }
}
