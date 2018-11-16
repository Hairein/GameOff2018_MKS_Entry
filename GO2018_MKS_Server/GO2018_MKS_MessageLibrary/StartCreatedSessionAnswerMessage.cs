﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class StartCreatedSessionAnswerMessage : GenericMessage
    {
        public bool Success;
        public string Details;

        public string opponentHandle;

        public StartCreatedSessionAnswerMessage()
        {
            Type = MessageType.startCreatedSessionAnswer;

            Success = false;
            Details = string.Empty;

            opponentHandle = string.Empty;
        }

        public StartCreatedSessionAnswerMessage(bool flag, string details, string newOpponentHandle)
        {
            Type = MessageType.startCreatedSessionAnswer;

            Success = flag;
            Details = details;

            opponentHandle = newOpponentHandle;
        }
    }
}
