using proje1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proje1.Controllers
{
    public class HomeController : Controller
    {
        List<Vehicle> vehicles = new List<Vehicle>();

        // GET: Home
        public ActionResult Index()
        {
            vehicles.Add(new Vehicle(1, 2));
            ViewData["data"] = vehicles;
            return View();
        }
    }
}