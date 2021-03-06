﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using web_api_for_good_transport.Models;

namespace web_api_for_good_transport.Controllers
{
    public class OrderController : ApiController
    {
        [System.Web.Http.Route("api/order/place_order")]
        [HttpPost]
        public HttpResponseMessage PlaceOrder([FromBody] Order order)
        {
            JObject obj = new JObject();
            try
            {
                DataTable result = DAL.RunStoreProc(order, "spPlaceOrder");
                Type user_type = order.GetType();
                order.order_id = Convert.ToInt32(result.Rows[0]["order_id"].ToString());
                return Request.CreateResponse(HttpStatusCode.OK, order);
            }
            catch (Exception ex)
            {
                 obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/order/get_pre_order_form_data")]
        [HttpGet]
        public HttpResponseMessage GetPreOrderFormData(bool is_cargo, bool is_measurement_unit, bool is_container, bool is_source, bool is_container_size, bool is_weight_catagory, bool is_payment_type)
        {
            JObject obj = new JObject();
            try
            {
                List<Cargo> cargo = new List<Cargo>();
                List<MeasurementUnit> measurement_unit = new List<MeasurementUnit>();
                List<Container> container = new List<Container>();
                List<Location> source = new List<Location>();
                List<ContainerSize> container_size = new List<ContainerSize>();
                List<WeightCatagory> weight_catagory = new List<WeightCatagory>();
                List<PaymentType> payment_type = new List<PaymentType>();

                if (is_cargo)
                {
                    cargo = get_cargo();
                    obj["cargo"] = JsonConvert.SerializeObject(cargo);
                }
                if (is_measurement_unit)
                {
                    measurement_unit = get_measurement_unit();
                    obj["measurement_unit"] = JsonConvert.SerializeObject(measurement_unit);
                }
                if (is_container)
                {
                    container = get_container();
                    obj["container"] = JsonConvert.SerializeObject(container);
                }
                if (is_source)
                {
                    source = get_sources();
                    obj["source"] = JsonConvert.SerializeObject(source);
                }
                if (is_container_size)
                {
                    container_size = get_container_size();
                    obj["container_size"] = JsonConvert.SerializeObject(container_size); 
                }
                if (is_weight_catagory)
                {
                    weight_catagory = get_weight_catagory();
                    obj["weight_catagory"] = JsonConvert.SerializeObject(weight_catagory);
                }
                if (is_payment_type)
                {
                    payment_type = get_payment_types();
                    obj["payment_type"] = JsonConvert.SerializeObject(payment_type);
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                JsonSerializerSettings se = new JsonSerializerSettings();

                return Request.CreateResponse(HttpStatusCode.OK, obj, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
            
        }

        [System.Web.Http.Route("api/order/get_all_orders_of_customer")]
        [HttpGet]
        public HttpResponseMessage GetAllOrdersOFCustomer(string customer_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = "spGetCustomerAllOrders";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@customer_id", customer_id);
                cmd.CommandType = CommandType.StoredProcedure;

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(DAL.Select(sql, cmd)), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/order/request_driver_for_order")]
        [HttpGet]
        public HttpResponseMessage RequestDriverForOrder(string driver_id, string order_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = "insert into tbl_order_detail (order_id, driver_id, status_id) VALUES (@order_id, @driver_id, @status)";

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@order_id", order_id);
                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                cmd.Parameters.AddWithValue("@status", 5);

                DAL.CreateUpdateDelete(sql, cmd);

                return Request.CreateResponse(HttpStatusCode.OK, "OK", Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/order/get_order_wrt_driver_and_status_id")]
        [HttpGet]
        public HttpResponseMessage get_order_wrt_driver_and_status_id(int driver_id, int status_id)
        {
            JObject obj = new JObject();
            try
            {
                //--pass specific status otherwise pass 0 to get all the data regardless of status.
                string sql = "spGetOrdersWrtDriverId";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                cmd.Parameters.AddWithValue("@status_id", status_id);
                string dt = DAL.SerializeDataTable(sql, cmd);

                return Request.CreateResponse(HttpStatusCode.OK, dt, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/order/get_requested_orders")]
        [HttpGet]
        public HttpResponseMessage get_requested_orders(int driver_id, int status_id)
        {
            JObject obj = new JObject();
            try
            {
                //--pass specific status otherwise pass 0 to get all the data regardless of status.
                string sql = "spGetOrdersWrtDriverId";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                cmd.Parameters.AddWithValue("@status_id", status_id);
                string dt = DAL.SerializeDataTable(sql, cmd);

                return Request.CreateResponse(HttpStatusCode.OK, dt, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/order/confirm_order")]
        [HttpGet]
        public HttpResponseMessage confirm_order(int driver_id, string order_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = "spConfirmOrder";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@driver_id", driver_id);
                cmd.Parameters.AddWithValue("@order_id", order_id);
                string dt = DAL.SerializeDataTable(sql, cmd);

                return Request.CreateResponse(HttpStatusCode.OK, dt, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        [System.Web.Http.Route("api/order/get_order_cost")]
        [HttpGet]
        public HttpResponseMessage get_order_cost(string source_id, string destination_id)
        {
            JObject obj = new JObject();
            try
            {
                string sql = @"select tbl_location_price.transport_location_id, location_price_id, coalesce([20ft_price], 0) as [20ft_price], coalesce([40ft_price], 0) as [40ft_price], coalesce(lcl_price, 0) as lcl_price from tbl_transport_location
                            inner join tbl_location_price
                            on tbl_transport_location.transport_location_id = tbl_location_price.transport_location_id
                            where tbl_transport_location.source_id = @source_id and 
                            (tbl_transport_location.D1 = @destination_id OR tbl_transport_location.D2 = @destination_id OR tbl_transport_location.D3 = @destination_id
                            OR tbl_transport_location.D4 = @destination_id OR tbl_transport_location.D5 = @destination_id OR tbl_transport_location.D6 = @destination_id
                            OR tbl_transport_location.D7 = @destination_id OR tbl_transport_location.D8 = @destination_id OR tbl_transport_location.D9 = @destination_id
                            OR tbl_transport_location.D10 = @destination_id)";

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@source_id", source_id);
                cmd.Parameters.AddWithValue("@destination_id", destination_id);

                string response = DAL.SerializeDataTable(sql, cmd);
                obj["success"] = response.Trim() == "[]" ? false : true;
                obj["data"] = response;
                return Request.CreateResponse(HttpStatusCode.OK, obj, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                obj["success"] = false;
                obj["data"] = ex.Message + " " + ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.InternalServerError, obj);
            }
        }

        #region private_methods

        private List<Cargo> get_cargo()
        {
            List<Cargo> cargo = new List<Cargo>();
            string sql = @"select * from tbl_cargo_type";
            DataTable dt = DAL.Select(sql);

            //return dt;
            foreach (DataRow item in dt.Rows)
            {
                Cargo c1 = new Cargo();
                c1.cargo_type_id = Convert.ToInt32(item["cargo_type_id"].ToString());
                c1.cargo_type = item["cargo_type"].ToString();
                cargo.Add(c1);
            }
            return cargo;
        }

        private List<MeasurementUnit> get_measurement_unit()
        {
            List<MeasurementUnit> measurement_unit = new List<MeasurementUnit>();
            string sql = @"select * from tbl_measurement_unit";
            DataTable dt = DAL.Select(sql);
            foreach (DataRow item in dt.Rows)
            {
                MeasurementUnit m1 = new MeasurementUnit();
                m1.unit_id = Convert.ToInt32(item["unit_id"].ToString());
                m1.unit_name = item["unit_name"].ToString();
                measurement_unit.Add(m1);
            }
            return measurement_unit;
        }

        private List<Container> get_container()
        {
            List<Container> container = new List<Container>();
            string sql = @"select * from tbl_container_type";
            DataTable dt = DAL.Select(sql);
            foreach (DataRow item in dt.Rows)
            {
                Container c1 = new Container();
                c1.container_type_id = Convert.ToInt32(item["container_type_id"].ToString());
                c1.container_type = item["container_type"].ToString();
                container.Add(c1);
            }
            return container;
        }

        private List<Location> get_sources()
        {
            List<Location> source = new List<Location>();
            string sql = @"spGetSources";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = sql;
            SqlConnection con = (SqlConnection)DAL.GetConnection("sql");
            con.Open();
            cmd.Connection = con;
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                Location s1 = new Location();
                s1.location_id = Convert.ToInt32(sdr["source_id"].ToString());
                s1.location_name = sdr["location_name"].ToString();
                s1.latitude = sdr["latitude"].ToString();
                s1.longitude = sdr["longitude"].ToString();

                source.Add(s1);
            }
            con.Close();
            return source;
        }

        private List<ContainerSize> get_container_size()
        {
            List<ContainerSize> containerSize = new List<ContainerSize>();
            string sql = @"select * from tbl_vehicle_type";
            DataTable dt = DAL.Select(sql);
            foreach (DataRow item in dt.Rows)
            {
                ContainerSize c1 = new ContainerSize();
                c1.vehicle_type_id = Convert.ToInt32(item["vehicle_type_id"].ToString());
                c1.vehicle_type = item["vehicle_type"].ToString();
                containerSize.Add(c1);
            }
            return containerSize;
        }

        private List<WeightCatagory> get_weight_catagory()
        {
            List<WeightCatagory> weightCatagory = new List<WeightCatagory>();
            string sql = @"select * from tbl_weight_catagory";
            DataTable dt = DAL.Select(sql);
            foreach (DataRow item in dt.Rows)
            {
                WeightCatagory c1 = new WeightCatagory();
                c1.weight_id = Convert.ToInt32(item["weight_id"].ToString());
                c1.weight_catagory = item["weight_catagory"].ToString();
                weightCatagory.Add(c1);
            }
            return weightCatagory;
        }

        private List<PaymentType> get_payment_types()
        {
            List<PaymentType> paymentType = new List<PaymentType>();
            string sql = @"select * from tbl_payment_type";
            DataTable dt = DAL.Select(sql);
            foreach (DataRow item in dt.Rows)
            {
                PaymentType p1 = new PaymentType();
                p1.payment_type_id= Convert.ToInt32(item["payment_type_id"].ToString());
                p1.payment_type = item["payment_type"].ToString();
                paymentType.Add(p1);
            }
            return paymentType;
        }
        #endregion
    }
}
