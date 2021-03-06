﻿using Core.Expansions;
using Core.Services.AppState;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Models;
using Core.Models.Hubs;
using Core.Services.DB.Actions;
using Core.Services.DB;

namespace Core.Services.Hubs
{
    [Authorize]
    public class GameHub : BaseHub
    {
        public static IHubContext<GameHub> HubContext { get; private set; }

        public static void SetHubContext(IHubContext<GameHub> context)
        {
            if (HubContext.IsNull())
                HubContext = context;
        }

        private readonly ActionsBuilder Builder;

        private AppDBContext DBContext => Builder.Context;

        public GameHub(ILogger<GameHub> logger, AppObjects objects, ActionsBuilder builder) : base(logger, objects)
        {
            Builder = builder;
        }

        public async Task GetGameInfo(long id)
        {
            await GetGameRoomObjectOrReturnNotExist(id, async obj =>
            {
                var roomInfo = obj.GetVueLobbyInfo();

                await Clients.Caller.SendAsync("InitGameInfo", roomInfo);
            });
        }

        public async Task Connect(IdObject<string> obj)
        {
            await GetGameRoomObjectOrReturnNotExist(obj.Id, async gobj =>
            {
                var user = GetUser().CreateUserStruct();

                if (gobj.LobbyInfo.IsPrivate)
                {
                    var validPaswd = gobj.LobbyInfo.Password;
                    if (validPaswd != obj.Obj)
                    {
                        await ReturnClientMessage(new ClientMessage(ClientMessageType.Error, "Пароль не подходит"));
                        return;
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("ClosePasswdWindow");
                    }
                }

                if (gobj.ContainsUser(user))
                    gobj[user] = Context.ConnectionId;
                else
                    gobj.AddUser(user, Context.ConnectionId);

                await ReturnClientMessage(new ClientMessage(ClientMessageType.Success, "Хей!"));
                await Clients.Caller.SendAsync("GetAllUsersInRoom");
            });
        }

        public async Task GetAllUsersInRoom(long id)
        {
            await GetGameRoomObjectOrReturnNotExistOrNonAuth(id, async obj =>
            {
                var users = obj
                .Where(x => x.Value.IsNotNull())
                .Select(x => new { Id = obj.GetId(x.Key), Name = x.Key.Name })
                .ToList();

                await Clients.Caller.SendAsync("InitUsersList", users);
            });
        }

        public async Task GetUserAva(IdObject<long> obj)
        {
            await GetGameRoomObjectOrReturnNotExistOrNonAuth(obj.Id, async gobj =>
            {
                var user = gobj.GetUser(obj.Obj);

                if (user.HasValue)
                {
                    var userInfo = await Builder.GetUserStateActions.GetAsyncIncludeUserConfigure(user.Value.Name, user.Value.FingerPrint);
                    if (userInfo.IsNotNull() && userInfo.UserConfigure.IsNotNull())
                    {
                        await Clients.Caller.SendAsync("SetUserAva", new { Id = obj.Obj, Ava = userInfo.UserConfigure.Base64Ava });                        
                    }
                }
            });
        }

        public async Task StartGame(long id)
        {
            await GetGameRoomObjectOrReturnNotExistOrNonAuth(id, async gobj =>
            {
                var user = GetUser().CreateUserStruct();

                if (gobj.Admin.Key.Equals(user))
                {
                    await gobj.GameLogic.SetNextState();
                }
            });
        }

        private async Task GetGameRoomObjectOrReturnNotExistOrNonAuth(long id, Func<GameRoom, Task> clbk)
        {
            await GetGameRoomObjectOrReturnNotExist(id, async obj => 
            {
                var user = GetUser().CreateUserStruct();

                if (!obj.ContainsUser(user))
                {
                    await ReturnClientMessage(new ClientMessage(ClientMessageType.Error, "Недостаточно прав"));
                }
                else
                    await clbk(obj);
            });
        }

        private async Task GetGameRoomObjectOrReturnNotExist(long id, Func<GameRoom, Task> clbk)
        {
            await TryCatchLogAsync(async () => 
            {
                var obj = Objects[id] as GameRoom;
                if (obj.IsNull())
                {
                    await ReturnClientMessage(new ClientMessage(ClientMessageType.Error, "Такой комнаты не существует"));
                    await SwitchClientPage("hub");
                }
                else
                    await clbk(obj);
            });
        }
    }
}
