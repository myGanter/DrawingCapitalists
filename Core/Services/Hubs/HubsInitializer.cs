using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Services.Hubs;

namespace Core.Services.Hubs
{
    public static class HubsInitializer
    {
        public static void InitializeContextsHub(IServiceProvider serviceProvider)
        {
            var roomsHubContext = serviceProvider.GetService<IHubContext<RoomsHub>>();
            RoomsHub.SetHubContext(roomsHubContext);

            var gameHubContext = serviceProvider.GetService<IHubContext<GameHub>>();
            GameHub.SetHubContext(gameHubContext);
        }
    }
}
