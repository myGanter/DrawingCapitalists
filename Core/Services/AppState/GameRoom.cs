using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Core.Expansions;
using Core.Services.Hubs;
using Core.Models.Hubs;
using Core.Models;

namespace Core.Services.AppState
{
    public class GameRoom : AppObject
    {
        public readonly CreateLobbyInfo LobbyInfo;

        public GameRoom(CreateLobbyInfo lobbyInfo)
        {
            LobbyInfo = lobbyInfo;

            OnConnectionAddOtherData += GameRoom_OnConnectionAddOtherData;
            OnConnectionRemoveOtherData += GameRoom_OnConnectionRemoveOtherData;
        }

        private void GameRoom_OnConnectionRemoveOtherData(UserStruct user, string connection)
        {
            var others = this.Where(x => !x.Key.Equals(user)).Select(x => x.Value);
            GameHub.HubContext.Clients.Clients(others).SendAsync("RemoveUser", new { Id = GetId(user), Name = user.Name });
        }

        private void GameRoom_OnConnectionAddOtherData(UserStruct user, string connection)
        {
            var others = this.Where(x => !x.Key.Equals(user)).Select(x => x.Value);
            GameHub.HubContext.Clients.Clients(others).SendAsync("AddUser", new { Id = GetId(user), Name = user.Name });
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
