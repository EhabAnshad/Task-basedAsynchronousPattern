using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServerClient<Order> _orderClient;
        private readonly IConfiguration _section;

        public HomeController(IServerClient<Order> orderClient, IConfiguration config)
        {
            _orderClient = orderClient;
            _section = config.GetSection("ExternalService");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Order order)
        {
            var results = _orderClient.PostAsync(order, _section["Path"]).Result;
           return Redirect("Home/Refresh/"+ results.OrderId);
        }

        [HttpGet]
        public ActionResult Refresh(Guid id)
        {
            var results = _orderClient.GetAsync(id.ToString(), _section["Path"]).Result;
            return results.OrderId == Guid.Empty ? PartialView("Index") : PartialView("Details", results);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
