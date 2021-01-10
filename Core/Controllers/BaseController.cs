using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Core.Models;
using Core.Expansions;
using System.Security.Claims;

namespace Core.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger Logger;

        protected const string FingerPrintClaimType = "FingerPrint";

        public BaseController(ILogger logger)
        {
            Logger = logger;
        }

        protected IActionResult TryCatchLog(Func<IActionResult> clbk, string msg = "Ошибка :c")
        {         
            try
            {
                return clbk();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, new EventId(), GetUser().CreateContainer(null), e, (u, ex) => ex.Message);

                return GetBadResult(msg ?? e.Message);
            }
        }

        protected async Task<IActionResult> TryCatchLog(Func<Task<IActionResult>> clbk, string msg = "Ошибка :c")
        {
            try
            {
                return await clbk();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, new EventId(), GetUser().CreateContainer(null), e, (u, ex) => ex.Message);

                return GetBadResult(msg ?? e.Message);
            }
        }

        protected User GetUser()
        {
            string ip = null;
            if (HttpContext.Connection.RemoteIpAddress.IsNotNull())
            {
                if (HttpContext.Connection.RemoteIpAddress.IsIPv4MappedToIPv6)
                    ip = HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                else
                    ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }

            return new User()
            {
                Ip = ip,
                Name = User.Identity?.Name,
                FingerPrint = User.Claims.FirstOrDefault(x => x.Type == FingerPrintClaimType)?.Value
            };
        }

        protected ClientMessage GetModelErrorsOrNull()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage))
                    .ToArray();

                return new ClientMessage(ClientMessageType.Error, errors);
            }

            return null;
        }

        protected IActionResult GetBadResult(ClientMessage message)
        {
            return BadRequest(message);
        }

        protected IActionResult GetBadResult(params string[] messages)
        {
            var resultMessage = new ClientMessage(ClientMessageType.Error, messages);

            return GetBadResult(resultMessage);
        }
    }
}
