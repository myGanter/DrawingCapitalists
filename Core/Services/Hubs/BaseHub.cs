using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Core.Expansions;
using Core.Models;
using Core.Services.Authentication;
using Core.Services.AppState;

namespace Core.Services.Hubs
{
    public abstract class BaseHub : Hub
    {
        protected readonly ILogger Logger;

        protected readonly AppObjects Objects;

        public BaseHub(ILogger logger, AppObjects objects)
        {
            Logger = logger;
            Objects = objects;
        }

        protected User GetUser()
        {
            var httpContext = Context.GetHttpContext();
            string ip = null;
            if (httpContext.IsNotNull() && httpContext.Connection.RemoteIpAddress.IsNotNull())
            {
                if (httpContext.Connection.RemoteIpAddress.IsIPv4MappedToIPv6)
                    ip = httpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                else
                    ip = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }

            return new User()
            {
                Ip = ip,
                Name = Context.User.Identity?.Name,
                FingerPrint = Context.User.Claims.FirstOrDefault(x => x.Type == AuthenticationHelper.FingerPrintClaimType)?.Value
            };
        }

        protected ClientMessage GetModelErrorsOrNull(object model)
        {
            var errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, new ValidationContext(model), errors, true))
            {
                return new ClientMessage(ClientMessageType.Error, errors.Select(x => x.ErrorMessage).ToArray());
            }

            return null;
        }

        protected async Task ReturnClientMessage(ClientMessage message)
        {
            await Clients.Caller.SendAsync("ShowClientMessage", message);
        }

        //public override async Task OnConnectedAsync()
        //{
        //    var us = Context.UserIdentifier;

        //    await base.OnConnectedAsync();
        //}

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = Context.ConnectionId;
            var obj = Objects[connection];
            if (obj.IsNotNull())
            {
                var user = GetUser();
                obj[user.CreateUserStruct()] = null;
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
