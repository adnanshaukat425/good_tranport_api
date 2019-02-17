using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class Vehicle
    {
        public int vehicle_id { get; set; }
        public int vehicle_type_id { get; set; }
        public int container_type_id { get; set; }
        public string vehicle_number { get; set; }
        public int transporter_id { get; set; }
    }
}