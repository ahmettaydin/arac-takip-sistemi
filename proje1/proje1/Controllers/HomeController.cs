using MongoDB.Bson;
using MongoDB.Driver;
using proje1.Database;
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
        private SqlConnection connection = new SqlConnection("Data Source=DESKTOP-JU7J8P7;Initial Catalog=Taxi;Integrated Security=True");
        private List<CustomerVehicles> cusVehicles;
        private Customer customer;

        private MongoDBContext dBContext;
        private IMongoCollection<Vehicle> mongoCollection;

        public HomeController()
        {
            dBContext = new MongoDBContext();
            mongoCollection = dBContext.database.GetCollection<Vehicle>("Vehicles");
        }

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
            cusVehicles = GetCustomerVehicles(customer.CusID);
            var vehicle1 = mongoCollection.Find(x => x.ID == cusVehicles[0].VehicleID).ToList();
            var vehicle2 = mongoCollection.Find(x => x.ID == cusVehicles[1].VehicleID).ToList();
            DateTime date1 = vehicle1[vehicle1.Count-1].Date;
            date1 = date1.AddMinutes(-30);
            DateTime date2 = vehicle2[vehicle2.Count - 1].Date;
            date2 = date2.AddMinutes(-30);
            var vehicle11 = mongoCollection.Find(x => x.Date >= date1 && x.ID == cusVehicles[0].VehicleID).ToList();
            var vehicle22 = mongoCollection.Find(x => x.Date >= date2 && x.ID == cusVehicles[1].VehicleID).ToList();
            int visitingID = int.Parse(Request.QueryString["visitingID"]);
            TempData["visitingID"] = visitingID;
            ViewData["customer"] = customer;
            ViewData["vehicle1"] = vehicle11;
            ViewData["vehicle2"] = vehicle22;
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

        private List<CustomerVehicles> GetCustomerVehicles(int cusID)
        {
            List<CustomerVehicles> customerVehicles = new List<CustomerVehicles>();
            connection.Open();
            string sql = "SELECT * FROM Vehicle WHERE cusID = @cusID";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("cusID", cusID);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                customerVehicles.Add(new CustomerVehicles(Convert.ToInt32(reader["vehicleID"]), Convert.ToInt32(reader["cusID"])));
            }
            connection.Close();
            return customerVehicles;
        }
    }
}
