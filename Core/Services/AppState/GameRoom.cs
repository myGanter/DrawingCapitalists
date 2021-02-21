using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Core.Expansions;
using Core.Services.Hubs;
using Core.Models.Hubs;
using Core.Models;
using Core.Services.Game;

namespace Core.Services.AppState
{
    public class GameRoom : AppObject
    {
        public readonly CreateLobbyInfo LobbyInfo;

        public readonly GameLogic GameLogic;

        public KeyValuePair<UserStruct, string> Admin { get; private set; }

        public GameRoom(CreateLobbyInfo lobbyInfo)
        {
            LobbyInfo = lobbyInfo;
            GameLogic = new GameLogic(this);

            OnConnectionAddOtherData += GameRoom_OnConnectionAddOtherData;
            OnConnectionRemoveOtherData += GameRoom_OnConnectionRemoveOtherData;
        }

        private void GameRoom_OnConnectionRemoveOtherData(UserStruct user, string connection)
        {
            var others = GetOthersConnections(user);
            GameHub.HubContext.Clients.Clients(others).SendAsync("RemoveUser", new { Id = GetId(user), Name = user.Name });
            RoomsHub.HubContext.Clients.All.SendAsync("UpdateLobby", GetVueLobbyInfo());

            if (Admin.Value == connection)
            {
                Admin = this.Where(x => x.Value.IsNotNull()).FirstOrDefault();

                if (Admin.Value.IsNotNull())
                    GameHub.HubContext.Clients.Client(Admin.Value).SendAsync("InstallAsAnAdmin");
            }
        }

        private void GameRoom_OnConnectionAddOtherData(UserStruct user, string connection)
        {
            var others = GetOthersConnections(user);
            GameHub.HubContext.Clients.Clients(others).SendAsync("AddUser", new { Id = GetId(user), Name = user.Name });
            RoomsHub.HubContext.Clients.All.SendAsync("UpdateLobby", GetVueLobbyInfo());

            if (Admin.Value.IsNull())
            {
                Admin = this.Where(x => x.Value.IsNotNull()).FirstOrDefault();
                GameHub.HubContext.Clients.Client(Admin.Value).SendAsync("InstallAsAnAdmin");
            }
        }

        public VueLobbyInfo GetVueLobbyInfo()
        {
            return new VueLobbyInfo()
            {
                Id = Id,
                IsPrivate = LobbyInfo.IsPrivate,
                Name = LobbyInfo.Name,
                PlayersCount = UsersCache.Count(x => x.Value.IsNotNull())
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
