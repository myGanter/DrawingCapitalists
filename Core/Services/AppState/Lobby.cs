using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Core.Expansions;
using Core.Services.Hubs;
using Core.Models.Hubs;

namespace Core.Services.AppState
{
    public class Lobby : AppObject
    {
        private readonly CreateLobbyInfo LobbyInfo;

        public Lobby(CreateLobbyInfo lobbyInfo)
        {
            LobbyInfo = lobbyInfo;
        }

        public VueLobbyInfo GetVueLobbyInfo()
        {
            return new VueLobbyInfo()
            {
                Id = Id,
                IsPrivate = LobbyInfo.IsPrivate,
                Name = LobbyInfo.Name,
                PlayersCount = Count
            };
        }

        public override bool IsEmpty()
        {
            lock (UsersCache)
            {
                return UsersCache.All(x => x.Value.IsNull());
            }
        }

        public override async Task NotifyDeath()
        {
            await RoomsHub.HubContext.Clients.All.SendAsync("RemoveLobby", Id);
        }
    }
}
