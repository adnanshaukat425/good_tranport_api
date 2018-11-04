using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class User
    {
        public int user_id { get; set; }
        public int user_type_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string cnic_number { get; set; }
        public string profile_picture { get; set; }
        public string password { get; set; }
    }
}