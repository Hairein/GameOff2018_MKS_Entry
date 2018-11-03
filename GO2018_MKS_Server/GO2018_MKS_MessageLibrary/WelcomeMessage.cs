using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class WelcomeMessage : GenericMessage
    {
        public string text;

        public WelcomeMessage()
        {
            Type = MessageType.welcome;

            text = "Welcome to the GO2018 Game Server";
        }

        public WelcomeMessage(string messageText)
        {
            Type = MessageType.welcome;

            text = messageText;
        }
    }
}
