using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class LoginMessage : GenericMessage
    {
        public string PlatformId;
        public string PlayerHandle;

        public LoginMessage()
        {
            Type = MessageType.login;

            PlatformId = string.Empty;
            PlayerHandle = string.Empty;
        }

        public LoginMessage(string id, string name)
        {
            Type = MessageType.login;

            PlatformId = id;
            PlayerHandle = name;
        }
    }
}
