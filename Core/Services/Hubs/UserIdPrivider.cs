using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Services.Authentication;

namespace Core.Services.Hubs
{
    public class UserIdPrivider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            var name = connection.User.Identity?.Name;
            var fingerPrint = connection.User.Claims.FirstOrDefault(x => x.Type == AuthenticationHelper.FingerPrintClaimType)?.Value;

            return name + fingerPrint;
        }
    }
}
