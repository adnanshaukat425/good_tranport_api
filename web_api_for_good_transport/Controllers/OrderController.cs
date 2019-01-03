using Newtonsoft.Json;
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
                order.order_id = Convert.ToInt32(result.Rows[0][0].ToString());
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
        public HttpResponseMessage GetPreOrderFormData()
        {
            JObject obj = new JObject();
            try
            {
                List<Cargo> cargo = get_cargo();
                List<MeasurementUnit> measurement_unit = get_measurement_unit();
                List<Container> container = get_container();
                List<Location> source = get_sources();
                List<ContainerSize> container_size = get_container_size();
                List<WeightCatagory> weight_catagory = get_weight_catagory();
                List<PaymentType> payment_type = get_payment_types();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                JsonSerializerSettings se = new JsonSerializerSettings();

                obj["cargo"] = JsonConvert.SerializeObject(cargo);
                obj["measurement_unit"] = JsonConvert.SerializeObject(measurement_unit);
                obj["container"] = JsonConvert.SerializeObject(container);
                obj["container_size"] = JsonConvert.SerializeObject(container_size);
                obj["weight_catagory"] = JsonConvert.SerializeObject(weight_catagory);
                obj["source"] = JsonConvert.SerializeObject(source);
                obj["payment_type"] = JsonConvert.SerializeObject(payment_type);

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
                DataTable dt = DAL.Select(sql, cmd);

                return Request.CreateResponse(HttpStatusCode.OK, dt, Configuration.Formatters.JsonFormatter);
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
