using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Expansions;

namespace Core.Models
{
    public class ClientMessage
    {
        public ClientMessageType MessageType { get; set; }

        public List<string> Messages { get; set; }

        public ClientMessage()
        {
            MessageType = ClientMessageType.Common;
            Messages = new List<string>();
        }

        public ClientMessage(ClientMessageType type, params string[] messages) : this()
        {
            MessageType = type;
            Messages.AddRange(messages.Where(x => !x.IsNullOrEmpty()));
        }        
    }
}
