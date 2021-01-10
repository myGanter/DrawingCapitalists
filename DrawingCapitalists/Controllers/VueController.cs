using DrawingCapitalists.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Core.Controllers;
using Core.Services.VueApp;
using Core.Services.DB.Actions;
using Core.Expansions;
using Microsoft.AspNetCore.Authorization;

namespace DrawingCapitalists.Controllers
{
    public class VueController : BaseController
    {
        private readonly VueTemplateService TemplateService;

        private readonly ActionsBuilder Builder;

        public VueController(ILogger<VueController> logger, VueTemplateService templateService, ActionsBuilder builder) : base(logger)
        {       
            TemplateService = templateService;
            Builder = builder;
        }        

        [HttpGet]
        public async Task<IActionResult> Index(string page)
        {
            await Builder.GetUserStateActions.TransactionAsync(async () => 
            {
                var o = await Builder.GetUserStateActions.GetAsync("test", "test2");
                if (o.IsNull())
                    await Builder.GetUserStateActions.AddAsync(new Core.Models.DB.UserState() { FingerPrint = "test2", Name = "test" });
            });

            Logger.LogWarning("test", "1", "2");

            if (page == null)
                return View();

            var newName = "_" + page;

            return PartialView("~/Views/VueComponents/" + newName + ".cshtml");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index2()
        {       
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
