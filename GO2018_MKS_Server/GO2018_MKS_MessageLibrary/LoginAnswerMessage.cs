using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class LoginAnswerMessage : GenericMessage
    {
        public bool Success;
        public string Details;

        public LoginAnswerMessage()
        {
            Type = MessageType.loginAnswer;

            Success = false;
            Details = string.Empty;
        }

        public LoginAnswerMessage(bool flag, string details)
        {
            Type = MessageType.loginAnswer;

            Success = flag;
            Details = details;
        }
    }
}
