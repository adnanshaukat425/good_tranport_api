﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class Order
    {
        public int order_id { get; set; }
        public int cargo_type_id { get; set; }
        public int container_type_id { get; set; }
        public int customer_id { get; set; }
        public int vehicle_type_id { get; set; }
        public string weight_catagory_id { get; set; }
        public string cargo_volume { get; set; }
        public int weight_unit_id { get; set; }
        public int source_id { get; set; }
        public int destination_id { get; set; }
        public bool is_labour_required { get; set; }
        public float labour_cost { get; set; }
        public int labour_quantity { get; set; }
        public string description { get; set; }
        public int payment_type_id { get; set; }
        public DateTime creation_datetime { get; set; }
        public DateTime order_datetime { get; set; }
    }
}
