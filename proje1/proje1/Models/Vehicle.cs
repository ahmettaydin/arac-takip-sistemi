using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proje1.Models
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public int CusID { get; set; }

        public Vehicle()
        {

        }

        public Vehicle(int vehicleID, int cusID)
        {
            VehicleID = vehicleID;
            CusID = cusID;
        }
    }
}