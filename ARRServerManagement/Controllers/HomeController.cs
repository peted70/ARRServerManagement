﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ARRServerManagement.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ARRServerManagement.Controllers
{
    class TokenResponse
    {
        public string AccessToken { get; set; }
    }


 
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string _arrAccountId = "";
        private string _arrAccountKey = "";

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;

            _arrAccountId = config.GetValue<string>("ARR:AccountId");
            if (string.IsNullOrEmpty(_arrAccountId))
            {
                throw new ArgumentException("ARR Account Id not set [set ARR:AccountId as a user secret]");
            }

            _arrAccountKey = config.GetValue<string>("ARR:AccountKey");
            if (string.IsNullOrEmpty(_arrAccountKey))
            {
                throw new ArgumentException("ARR Account Key not set [set ARR:AccountKey as a user secret]");
            }
        }

        public ActionResult StopServer(string sessionId)
        {



            return View("Home");
        }

        async Task<string> GetTokenAsync()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://sts.mixedreality.azure.com");


            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                $"{_arrAccountId}:{_arrAccountKey}");

            string uri = $"accounts/{_arrAccountId}/token";

            var resp = await httpClient.GetStringAsync(uri);

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(resp);

            return tokenResponse.AccessToken;
        }

        public async Task<IActionResult> Index()
        {
            var token = await GetTokenAsync();

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://remoterendering.westeurope.mixedreality.azure.com");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string response = null;
            string uri = $"/v1/accounts/{_arrAccountId}/sessions";
            try
            {
                response = await httpClient.GetStringAsync(uri);
            }
            catch (HttpRequestException hre)
            {
                return View(hre.Message);
            }
            catch (ArgumentNullException ane)
            {
                return View(ane.Message);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }

            if (string.IsNullOrEmpty(response))
            {
                return View("Error - empty resposne");
            }

            var sessionRoot = JsonConvert.DeserializeObject<SessionRoot>(response);

            return View(new SessionsModel { Root = sessionRoot });
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
