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

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                JsonSerializerSettings se = new JsonSerializerSettings();

                obj["cargo"] = JsonConvert.SerializeObject(cargo);
                obj["measurement_unit"] = JsonConvert.SerializeObject(measurement_unit);
                obj["container"] = JsonConvert.SerializeObject(container);

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

        #endregion
    }
}