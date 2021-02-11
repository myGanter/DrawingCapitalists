using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Core.Models;
using Core.Expansions;
using Core.Exceptions;
using Core.Services.Authentication;
using System.Security.Claims;

namespace Core.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger Logger;

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
                LogForTryCatch(e);

                return GetBadResult(msg.IsNull() ? e is ClientException ? e.Message : "Ошибка :c" : msg);
            }
        }

        protected async Task<IActionResult> TryCatchLogAsync(Func<Task<IActionResult>> clbk, string msg = "Ошибка :c")
        {
            try
            {
                return await clbk();
            }
            catch (Exception e)
            {
                LogForTryCatch(e);

                return GetBadResult(msg.IsNull() ? e is ClientException ? e.Message : "Ошибка :c" : msg);
            }
        }      
        
        private void LogForTryCatch(Exception e)
        {
            var loggerT = Logger.GetType();

            Logger.WriteLog(LogLevel.Error, GetUser().CreateContainer(null, GetRequestId()), e, 
                (u, ex) => HttpContext.Request.Host + HttpContext.Request.Path, //url
                loggerT.IsGenericType ? $"{loggerT.GetGenericArguments()[0].Name}.???.TryCatchLogAsync" : "BaseController.TryCatchLogAsync");
        }

        protected string GetRequestId() 
        {
            return Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }

        protected bool UserIsAuthenticated()
        {
            return User.Identity?.IsAuthenticated ?? false; 
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
                FingerPrint = User.Claims.FirstOrDefault(x => x.Type == AuthenticationHelper.FingerPrintClaimType)?.Value
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
