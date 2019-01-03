using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;

namespace web_api_for_good_transport.Models
{
    enum CMD_TYPES
    {
        INSERT, UPDATE
    }
    public class DAL
    {
        public static DbConnection GetConnection(string server_type)
        {
            string connection_string = "";
            DbConnection con = null;
            if (server_type == "sql")
            {
                connection_string = @"Data Source= .;Initial Catalog= SmartTransport;Integrated Security=true;";
                //connection_string = @"Server=den1.mssql8.gear.host;Database=smarttransport1;User Id=smarttransport1;Password=Hp6ssx64-8-0;";
                SqlConnection sql_con = new SqlConnection(connection_string);
                con = sql_con;
            }
            else if (server_type == "access")
            {
                connection_string = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Adnan\Documents\Visual Studio 2015\Projects\Api\SampleFyp.accdb;Persist Security Info=False;";
                OleDbConnection oledb_con = new OleDbConnection(connection_string);
                con = oledb_con;
            }
            return con;
        }
        
        //public static DataTable Select(string sql, OleDbCommand cmd = null)
        //{
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand();
        //    }

        //    DataTable dt = new DataTable();
        //    OleDbConnection con = (OleDbConnection)GetConnection("access");
        //    con.Open();
        //    cmd.CommandText = sql;
        //    cmd.Connection = con;
        //    OleDbDataAdapter sda = new OleDbDataAdapter(cmd);
        //    sda.Fill(dt);
        //    con.Close();
        //    return dt;
        //}
        
        public static DataTable Select(string sql, SqlCommand cmd = null)
        {
            if (cmd == null)
            {
                cmd = new SqlCommand();
            }

            DataTable dt = new DataTable();
            SqlConnection con = (SqlConnection)GetConnection("sql");
            con.Open();
            cmd.CommandText = sql;
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            con.Close();
            return dt;
        }

        //public static string SerializeDataTable(string sql, OleDbCommand cmd = null)
        //{
        //    DataTable dt = Select(sql, cmd);
        //    return JsonConvert.SerializeObject(dt);
        //}
        
        public static string SerializeDataTable(string sql, SqlCommand cmd = null)
        {
            return SerializeDataTable(Select(sql, cmd));
        }

        public static string SerializeDataTable(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = new Dictionary<string, object>();

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col].ToString());
                }
                rows.Add(row);
            }

            return serializer.Serialize(rows);
        }

        public static string CreateUpdateDelete(string sql, SqlCommand cmd = null)
        {
            if (cmd == null)
            {
                cmd = new SqlCommand();
            }

            SqlConnection con = (SqlConnection)GetConnection("sql");
            cmd.CommandText = sql;
            cmd.Connection = con;
            con.Open();
            int rows_effected = cmd.ExecuteNonQuery();
            con.Close();
            return rows_effected.ToString();
        }

        public static string CreateUpdateDelete(string sql, OleDbCommand cmd = null)
        {
            if (cmd == null)
            {
                cmd = new OleDbCommand();
            }

            OleDbConnection con = (OleDbConnection)GetConnection("access");
            cmd.CommandText = sql;
            cmd.Connection = con;
            con.Open();
            int rows_effected = cmd.ExecuteNonQuery();
            con.Close();
            return rows_effected.ToString();
        }

        public static object SelectScalar(string sql, SqlCommand cmd)
        {
            SqlConnection con = (SqlConnection)GetConnection("sql");
            cmd.CommandText = sql;
            cmd.Connection = con;
            con.Open();
            object result = cmd.ExecuteScalar();
            con.Close();
            return result;
        }

        public static string get_sql(string cmd_type, string table_name, object obj, string[] restricted_col, string where_clause, out SqlCommand cmd)
        {
            cmd = new SqlCommand();
            if (cmd_type.Trim().ToLower() == "insert")
            {
                return get_insert_sql(table_name, obj, restricted_col, out cmd);
            }
            else if (cmd_type.Trim().ToLower() == "update")
            {
                return get_update_sql(table_name, obj, restricted_col, where_clause, out cmd);
            }
            return null;
        }

        private static string get_insert_sql(string table_name, object obj, string[] restricted_col, out SqlCommand cmd)
        {
            cmd = new SqlCommand();
            string sql_keys = "";
            string sql_values = "";
            Type user_type = obj.GetType();
            foreach (PropertyInfo prop in user_type.GetProperties())
            {
                if (prop.CanRead)
                {
                    string name = prop.Name;
                    if (!restricted_col.Contains(name))
                    {
                        string value = prop.GetValue(obj, null).ToString();
                        sql_keys += name + ",";
                        sql_values += "@" + name + ",";
                        cmd.Parameters.AddWithValue("@" + name, value);   
                    }
                }
            }
            sql_keys = sql_keys.Substring(0, sql_keys.Length - 1);
            sql_values = sql_values.Substring(0, sql_values.Length - 1);

            string sql = "INSERT INTO " + table_name + " (" + sql_keys + ") VALUES (" + sql_values + ")";
            return sql;
        }

        private static string get_update_sql(string table_name, object obj, string[] restricted_col, string where_clause, out SqlCommand cmd)
        {
            cmd = new SqlCommand();
            string sql_keys = "";
            Type user_type = obj.GetType();
            foreach (PropertyInfo prop in user_type.GetProperties())
            {
                if (prop.CanRead)
                {
                    //update tbl_name set name = @name, asdlfd = @asdf,
                    string name = prop.Name;
                    if (!restricted_col.Contains(name))
                    {
                        string value = prop.GetValue(obj, null).ToString();
                        sql_keys += name + "=" + "@" + name + ",";
                        //sql_values += "@" + name + ",";
                        cmd.Parameters.AddWithValue("@" + name, value);
                    }
                }
            }
            sql_keys = sql_keys.Substring(0, sql_keys.Length - 1);

            string sql = "UPDATE " + table_name + " set " + sql_keys + " WHERE " + where_clause;
            return sql;
        }

        public static object get_mapped_object(List<object> obj, DataTable dt, object resultant_object)
        {
            Type users = obj.GetType();
            
            for (int i = 0; i < obj.Count; i++)
            {
                int count = 0;
                obj[i] = new Object();
                PropertyInfo[] properties = obj[i].GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    property.SetValue(obj[i], dt.Rows[count++][property.Name]);
                }
            }
            return obj;
        }

        public static DataTable RunStoreProc(object obj, string stored_proc)
        {
            SqlCommand cmd = new SqlCommand();
            string[] restricted_col = new string[] { "order_id" };
            Type user_type = obj.GetType();
            foreach (PropertyInfo prop in user_type.GetProperties())
            {
                if (prop.CanRead)
                {
                    string name = prop.Name;
                    if (!restricted_col.Contains(name))
                    {
                        string value = prop.GetValue(obj, null).ToString();
                        if (prop.PropertyType == typeof(DateTime))
                        {
                            SqlParameter par = new SqlParameter("@" + name, SqlDbType.DateTime);
                            par.Value = value;
                            cmd.Parameters.Add(par);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + name, value);
                        }
                    }
                }
            }
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            return DAL.Select(stored_proc, cmd);
        }
    }
}