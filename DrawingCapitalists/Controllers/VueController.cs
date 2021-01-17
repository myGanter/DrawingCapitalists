using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Core.Controllers;
using Core.Services.VueApp;
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

        private AppDBContext DBContext => Builder.Context;

        public VueController(ILogger<VueController> logger, VueTemplateService templateService, ActionsBuilder builder) : base(logger)
        {       
            TemplateService = templateService;
            Builder = builder;
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
                            FirstPage = page.IsNullOrEmpty() ? "Hub" : page
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

                    return PartialView(TemplateService.GetComponentPath(normPageName));
                }
                else if (!page.IsNullOrEmpty() && !useLayout)
                {
                    return PartialView(TemplateService.GetComponentPath("_Login"));
                }

                return View(new VueConfig() 
                {
                    FirstPage = "Login"
                });
            });            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return GetBadResult("Произошло что-то очень плохое :c");
        }
    }
}
