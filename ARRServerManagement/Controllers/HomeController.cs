﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ARRServerManagement.Models;
using Microsoft.Extensions.Configuration;

namespace ARRServerManagement.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IModelAccessor _modelAccessor;
        private readonly IARRService _arrService;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, 
                              IModelAccessor modelAccessor, IARRService arrService)
        {
            _logger = logger;
            _modelAccessor = modelAccessor;
            _arrService = arrService;
        }

        public ActionResult CreateServer()
        {
            return View("Create", new CreateModel());
        }

        public async Task<ActionResult> Create(SessionDescriptor session)
        {
            await _arrService.CreateSessionAsync(session); 
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> StopServer(string sessionId)
        {
            await _arrService.StopServerAsync(sessionId);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            Response.Headers.Add("Refresh", "10");

            var sessions = await _arrService.GetSessionsAsync(); 
            return View(sessions);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
