using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SiteServer.Models;


namespace SiteServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Log(string search, string firstHit)
        {
            try
            {
                dynamic firstHitObject = JObject.Parse(firstHit);
                JObject output = new JObject();
                output.Add("searchTerm", search);
                output.Add("firstResult", firstHitObject);
                System.IO.File.AppendAllText("logfile.json", $"{output.ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}");
                Response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch
            {
                System.IO.File.AppendAllText("logfile.json", $"Bad Request {Environment.NewLine}");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
