using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Hubs
{
    public class VueLobbyInfo
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsPrivate { get; set; }

        public int PlayersCount { get; set; }
    }
}
