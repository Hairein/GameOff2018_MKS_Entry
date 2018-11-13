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
        public string Reason;

        public LoginAnswerMessage()
        {
            Success = false;
            Reason = string.Empty;
        }

        public LoginAnswerMessage(bool flag, string reason)
        {
            Success = flag;
            Reason = reason;
        }
    }
}
