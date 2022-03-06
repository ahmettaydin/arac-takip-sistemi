﻿using proje1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proje1.Controllers
{
    public class LoginController : Controller
    {
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-JU7J8P7;Initial Catalog=Taxi;Integrated Security=True");
        List<Customer> customers = new List<Customer>();
        Customer customer = null;
        DateTime now;

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string psw)
        {
            connection.Open();
            string sql = "SELECT * from Customer";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
       
            while(reader.Read())
            {
                customers.Add(new Customer(Convert.ToInt32(reader["cusID"]), reader["cusName"].ToString(), reader["cusLastName"].ToString(), reader["cusEmail"].ToString(), reader["cusPassword"].ToString(), Convert.ToInt32(reader["suspended"]), Convert.ToInt32(reader["incorrect"])));
            }

            connection.Close();

            var u = customers.FirstOrDefault(x => x.CusEmail == email && x.CusPassword == psw);

            if(u != null)
            {
                if(u.Suspended != 1)
                {
                    now = DateTime.Now;
                    customer = new Customer(u.CusID, u.CusName, u.CusLastName, u.CusEmail, u.CusPassword, u.Suspended, u.Incorrect);
                    InsertLogin(customer.CusID, now);
                    string url =
                        string.Format("/home/index?cusID={0}" +
                        "&cusName={1}" +
                        "&cusLastName={2}" +
                        "&cusEmail={3}" +
                        "&cusPsw={4}" +
                        "&suspended={5}" +
                        "&incorrect={6}" +
                        "&visitingID={7}",
                        u.CusID,u.CusName,u.CusLastName,u.CusEmail,u.CusPassword,u.Suspended,u.Incorrect,GetVisitingID());
                    return Redirect(url);
                }
                else
                {
                    ViewBag.LoginError = "Your account has been suspended.";
                    return View();
                }  
            }
            else
            {
                var v = customers.FirstOrDefault(x => x.CusEmail == email);
                if(v != null)
                {
                    UpdateIncorrect(email);
                    CheckIncorrect(email);
                }
                customers.Clear();
                ViewBag.LoginError = "Incorrect authentication credentials!";
                return View();
            }
        }

        private void UpdateIncorrect(string email)
        {
            connection.Open();
            string sql = "UPDATE Customer SET incorrect = incorrect - 1 WHERE cusEmail = @email";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("email", email);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private void CheckIncorrect(string email)
        {
            connection.Open();
            int incorrect = 3;
            string sql = "SELECT incorrect FROM Customer WHERE cusEmail = @email";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("email", email);
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                incorrect = Convert.ToInt32(reader["incorrect"]);
            }
            connection.Close();
            if (incorrect <= 0)
            {
                UpdateSuspended(email);
            }
        }
        
        private void UpdateSuspended(string email)
        {
            connection.Open();
            string sql = "UPDATE Customer SET suspended = 1 WHERE cusEmail = @email";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("email", email);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private void InsertLogin(int cusID, DateTime now)
        {
            connection.Open();
            string sql = "INSERT INTO VisitingTime (cusID, login) VALUES (@cusID, @now)";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("cusID", cusID);
            cmd.Parameters.AddWithValue("now", now);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private int GetVisitingID()
        {
            connection.Open();
            string sql = "SELECT MAX(visitingID) FROM VisitingTime";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            int visitingID = -1;
            while(reader.Read())
            {
                visitingID = Convert.ToInt32(reader[0]);
            }
            connection.Close();
            return visitingID;
        }
    }
}