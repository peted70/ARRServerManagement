using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ARRServerManagement.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;

namespace ARRServerManagement.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IModelAccessor _modelAccessor;
        private readonly IARRService _arrService;

        private readonly CreateModel _cm = new CreateModel();

        public HomeController(ILogger<HomeController> logger, IModelAccessor modelAccessor, 
            IARRService arrService)
        {
            _logger = logger;
            _modelAccessor = modelAccessor;
            _arrService = arrService;
        }

        public async Task<ActionResult> CreateServer()
        {
            _cm.Containers = await _modelAccessor.GetContainersAsync();
            return View("Create", _cm);
        }

        public async Task<IEnumerable<CloudBlob>> GetBlobs()
        {
            var ret = new List<CloudBlob>();
            if (_cm != null && _cm.SelectedContainer != null)
            {
                ret = await _modelAccessor.ListBlobsAsync(_cm.SelectedContainer, ".ezArchive");
            }
            return ret;
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

        public async Task<ActionResult> ExtendServer(string sessionId)
        {
            await _arrService.ExtendSessionAsync(sessionId);
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
