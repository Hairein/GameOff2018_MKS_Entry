﻿using GO2018_MKS_MessageLibrary;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

namespace GO2018_MKS_Server
{
    public class ActiveSessionInfo
    {
        public ConnectedClientInfo player1 = null;
        public ConnectedClientInfo player2 = null;

        public string MapName = string.Empty;
        public int DurationSeconds = 0;

        public SessionState State;

        public float SessionTimeRemainingSeconds = 0.0f;

        public int ReadyStateCounter = 0;

        public CollectSessionUpdateAnswers CollectSessionUpdateAnswers = new CollectSessionUpdateAnswers();
        private SessionUpdateAnswerMessage sessionUpdateAnswerMessage = new SessionUpdateAnswerMessage();

        private float scoreMultiplicationFactor = 1.0f;

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

            SessionTimeRemainingSeconds = (float)DurationSeconds;
        }

        public void Update(float deltaTime)
        {
            SessionTimeRemainingSeconds -= (deltaTime / 1000.0f); // Convert ms to seconds before reduction

            CollectSessionUpdateAnswers.SessionTimeLeft = SessionTimeRemainingSeconds;

            // Calculate scores
            foreach (var unitResource in CollectSessionUpdateAnswers.Player1UnitResourceStates)
            {
                if (unitResource.UnitType == UnitType.breeder)
                {
                    if (unitResource.FoodResourceCount > CollectSessionUpdateAnswers.PreviousPlayer1BreederResources)
                    {
                        CollectSessionUpdateAnswers.Player1Score += (unitResource.FoodResourceCount - CollectSessionUpdateAnswers.PreviousPlayer1BreederResources) * scoreMultiplicationFactor;
                    }

                    CollectSessionUpdateAnswers.PreviousPlayer1BreederResources = unitResource.FoodResourceCount;
                    break;
                }
            }

            foreach (var unitResource in CollectSessionUpdateAnswers.Player2UnitResourceStates)
            {
                if (unitResource.UnitType == UnitType.breeder)
                {
                    if (unitResource.FoodResourceCount > CollectSessionUpdateAnswers.PreviousPlayer2BreederResources)
                    {
                        CollectSessionUpdateAnswers.Player2Score += (unitResource.FoodResourceCount - CollectSessionUpdateAnswers.PreviousPlayer2BreederResources) * scoreMultiplicationFactor;
                    }

                    CollectSessionUpdateAnswers.PreviousPlayer2BreederResources = unitResource.FoodResourceCount;
                    break;
                }
            }

            // Copy over
            if (player1 != null && player2 != null)
            {
                sessionUpdateAnswerMessage.Convert(CollectSessionUpdateAnswers);

                player1.AddMessage(sessionUpdateAnswerMessage);
                player2.AddMessage(sessionUpdateAnswerMessage);
            }
        }
    }
}
