using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Core.Expansions;
using Core.Models.DTO.Authentication;
using Core.Models.DB;
using Core.Services.DB.Actions;
using Core.Services.DB;
using Microsoft.AspNetCore.Authorization;

namespace Core.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly ActionsBuilder Builder;

        private AppDBContext DBContext => Builder.Context;

        public AuthenticationController(ILogger<AuthenticationController> logger, ActionsBuilder builder) : base(logger)
        {
            Builder = builder;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthenticateDTO data)
        {
            return await TryCatchLogAsync(async () => 
            {
                var modelErrors = GetModelErrorsOrNull();
                if (modelErrors.IsNotNull())
                {
                    Logger.WriteLog(LogLevel.Warning, GetUser().CreateContainer(data), null, 
                        (o, e) => $"AuthenticateDTO не валидна.\n {o.Object.JsonSerialize()}", 
                        "AuthenticationController.Login");

                    return GetBadResult(modelErrors);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, data.Name),
                    new Claim(FingerPrintClaimType, data.FingerPrint)
                };

                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

                var userContext = await Builder.GetUserStateActions.GetAsync(data.Name, data.FingerPrint);
                if (userContext.IsNotNull())
                {
                    userContext.LoginTime = DateTime.Now;

                    await DBContext.SaveChangesAsync();
                }
                else
                {
                    await Builder.GetUserStateActions.AddAsync(new UserState() 
                    { 
                        Ip = GetUser().Ip, 
                        FingerPrint = data.FingerPrint, 
                        Name = data.Name, 
                        LoginTime = DateTime.Now 
                    });
                }                

                Logger.WriteLog(LogLevel.Information, GetUser().CreateContainer(data), null, 
                    (o, e) => $"Успешная аутентификация.\n {o.Object.JsonSerialize()}", 
                    "AuthenticationController.Login");

                return Ok();
            });            
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return await TryCatchLogAsync(async () =>
            {
                var user = GetUser();                

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var userContext = await Builder.GetUserStateActions.GetAsync(user.Name, user.FingerPrint);
                if (userContext.IsNotNull())
                {
                    userContext.LogoutTime = DateTime.Now;
                    userContext.IsActive = false;

                    await DBContext.SaveChangesAsync();
                }

                Logger.WriteLog(LogLevel.Information, user.CreateContainer(null), null, 
                    (o, e) => "Сделал Logout.", 
                    "AuthenticationController.Logout");

                return Ok();
            });
        }
    }
}
