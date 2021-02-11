using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Core.Services.AppState;
using Core.Expansions;
using Core.Models.Hubs;

namespace Core.Services.Hubs
{
    [Authorize]
    public class RoomsHub : BaseHub
    {
        public static IHubContext<RoomsHub> HubContext { get; private set; }

        public static void SetHubContext(IHubContext<RoomsHub> context)
        {
            if (HubContext.IsNull())
                HubContext = context;
        }

        public RoomsHub(ILogger<RoomsHub> logger, AppObjects objects) : base(logger, objects)
        {            
        }

        public async Task CreateNewLobby(CreateLobbyInfo lobbyInfo)
        {
            await TryCatchLogAsync(async () => 
            {
                var modelErrors = GetModelErrorsOrNull(lobbyInfo);
                if (modelErrors.IsNotNull())
                {
                    await ReturnClientMessage(modelErrors);
                    return;
                }

                var room = new GameRoom(lobbyInfo);
                var id = Objects.AddObject(room);
                var us = GetUser();
                room.AddUser(us.CreateUserStruct(), null);
                var vueLobbyInfo = room.GetVueLobbyInfo();

                await Clients.Others.SendAsync("AddLobby", vueLobbyInfo);
                await Clients.Caller.SendAsync("GoToLobby", new { Id = id, Password = lobbyInfo.Password } );
            });            
        }

        public override async Task OnConnectedAsync()
        {
            var rooms = Objects
                .Where(x => x.Value is GameRoom)
                .Select(x => (x.Value as GameRoom).GetVueLobbyInfo())
                .ToList();

            await Clients.Caller.SendAsync("InitLobbyList", rooms);

            await base.OnConnectedAsync();
        }
    }
}
