using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DrawingCapitalists.Services;
using Microsoft.AspNetCore.Diagnostics;

using Core.Controllers;
using Core.Services.DB.Actions;
using Core.Services.DB;
using Core.Expansions;
using Core.Models;

using DrawingCapitalists.Models;

namespace DrawingCapitalists.Controllers
{
    public class VueController : BaseController
    {
        private readonly VueTemplateService TemplateService;

        private readonly ActionsBuilder Builder;

        private readonly HttpQueryParamsService HttpQueryParams;

        private readonly VueModelCreatorService VueModelCreator;

        private AppDBContext DBContext => Builder.Context;

        public VueController(ILogger<VueController> logger, 
            VueTemplateService templateService, 
            ActionsBuilder builder, 
            HttpQueryParamsService httpQueryParams,
            VueModelCreatorService vueModelCreator) : base(logger)
        {       
            TemplateService = templateService;
            Builder = builder;
            HttpQueryParams = httpQueryParams;
            VueModelCreator = vueModelCreator;
        }        

        [HttpGet]
        public IActionResult Index(string page, bool useLayout = true)
        {
            return TryCatchLog(() => 
            {
                Logger.WriteLog(LogLevel.Information, GetUser().CreateContainer(null, GetRequestId()), null,
                    (x, ex) => $"page = {page}; useLayout = {useLayout}",
                    "VueController.Index");

                if (UserIsAuthenticated())
                {
                    if (useLayout)
                        return View(new VueConfig()
                        {
                            FirstPage = page.IsNullOrEmpty() ? "hub" : $"{page}{HttpQueryParams.GetQueryStr("useLayout")}"
                        });                                         

                    var normPageName = TemplateService.NormalizeTemplateName(page);
                    if (!TemplateService.TemplateExist(normPageName))
                    {
                        var msg = $"Страницы {page} не существует";

                        Logger.WriteLog(LogLevel.Warning, GetUser().CreateContainer(null, GetRequestId()), null,
                            (x, ex) => msg,
                            "VueController.Index");

                        return GetBadResult(msg);
                    }

                    return PartialView(TemplateService.GetComponentPath(normPageName)/*, VueModelCreator.CreateModelObjectOrNull(normPageName)*/);
                }
                else if (!page.IsNullOrEmpty() && !useLayout)
                {
                    if (page.ToLower() == "login")
                        return PartialView(TemplateService.GetComponentPath("_Login"));
                    else
                        return GetBadResult("Недостаточно прав, возможно стоит проверить куки");                    
                }

                return View(new VueConfig() 
                {
                    FirstPage = "login"
                });
            }, null);            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var requestId = GetRequestId();

            Logger.WriteLog(LogLevel.Error, GetUser().CreateContainer(null, requestId), exception?.Error,
                (u, ex) => HttpContext.Request.Host + HttpContext.Request.Path, //url
                "VueController.Error");

            return GetBadResult("Произошло что-то очень плохое :c", $"Id запроса: '{requestId}'");
        }
    }
}
