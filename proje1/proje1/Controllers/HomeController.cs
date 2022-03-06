using proje1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proje1.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-JU7J8P7;Initial Catalog=Taxi;Integrated Security=True");
        List<Vehicle> vehicles = new List<Vehicle>();
        Customer customer;

        // GET: Home
        public ActionResult Index()
        {
            customer = new Customer(
                int.Parse(Request.QueryString["cusID"]),
                Request.QueryString["cusName"],
                Request.QueryString["cusLastName"],
                Request.QueryString["cusEmail"],
                Request.QueryString["cusPsw"],
                int.Parse(Request.QueryString["suspended"]),
                int.Parse(Request.QueryString["incorrect"]));
            int visitingID = int.Parse(Request.QueryString["visitingID"]);
            TempData["visitingID"] = visitingID;
            vehicles.Add(new Vehicle(1, 1));
            vehicles.Add(new Vehicle(2, 1));
            ViewData["customer"] = customer;
            ViewData["vehicles"] = vehicles;
            return View();
        }

        public ActionResult Logout()
        {
            DateTime now = DateTime.Now;
            UpdateLogout(Convert.ToInt32(TempData["visitingID"]), now);
            return RedirectToAction("Index", "Login");
        }

        private void UpdateLogout(int visitingID, DateTime now)
        {
            connection.Open();
            string sql = "UPDATE VisitingTime SET logout = @now WHERE visitingID = @visitingID";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("now", now);
            cmd.Parameters.AddWithValue("visitingID", visitingID);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}