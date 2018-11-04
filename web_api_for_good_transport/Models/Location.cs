using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class Location
    {
        public int location_id { get; set; }
        public string location_name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}