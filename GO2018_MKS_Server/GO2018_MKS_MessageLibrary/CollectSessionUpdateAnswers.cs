using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class CollectSessionUpdateAnswers : GenericMessage
    {
        public float SessionTimeLeft;

        public List<UnitNavigationCommand> Player1UnitNavigationCommands = new List<UnitNavigationCommand>();
        public List<UnitNavigationCommand> Player2UnitNavigationCommands = new List<UnitNavigationCommand>();

        public List<UnitResourceState> Player1UnitResourceStates = new List<UnitResourceState>();
        public List<UnitResourceState> Player2UnitResourceStates = new List<UnitResourceState>();

        public List<MineResourceState> MineResourceStates = new List<MineResourceState>();

        public List<BarricadeResourceState> BarricadeResourceStates = new List<BarricadeResourceState>();

        public float PreviousPlayer1BreederResources = 300.0f;
        public float Player1Score = 0.0f;

        public float PreviousPlayer2BreederResources = 300.0f;
        public float Player2Score = 0.0f;

        public CollectSessionUpdateAnswers()
        {
            Reset();
        }

        public void Reset()
        {
            SessionTimeLeft = 0.0f;

            Player1UnitNavigationCommands.Clear();
            Player2UnitNavigationCommands.Clear();

            Player1UnitResourceStates.Clear();
            Player2UnitResourceStates.Clear();

            MineResourceStates.Clear();

            BarricadeResourceStates.Clear();
        }

        public void AddPlayer1UnitNavigationCommand(UnitNavigationCommand newCommand)
        {
            Player1UnitNavigationCommands.Add(newCommand);
        }

        public void AddPlayer2UnitNavigationCommand(UnitNavigationCommand newCommand)
        {
            Player2UnitNavigationCommands.Add(newCommand);
        }

        public void AddPlayer1UnitResourceStates(UnitResourceState newState)
        {
            Player1UnitResourceStates.Add(newState);
        }

        public void AddPlayer2UnitResourceStates(UnitResourceState newState)
        {
            Player2UnitResourceStates.Add(newState);
        }

        public void AddMineResourceState(MineResourceState newState)
        {
            MineResourceStates.Add(newState);
        }

        public void AddBarricadeResourceState(BarricadeResourceState newState)
        {
            BarricadeResourceStates.Add(newState);
        }
    }
}
