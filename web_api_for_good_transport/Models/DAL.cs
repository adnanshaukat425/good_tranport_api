using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class DAL
    {
        public static DbConnection get_connection(string server_type)
        {
            string connection_string = "";
            DbConnection con = null;
            if (server_type == "sql")
            {
                connection_string = @"Data Source= .;Initial Catalog= SmartTransport;Integrated Security=true;";
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
        public static DataTable select(string sql, OleDbCommand cmd = null)
        {
            if (cmd == null)
            {
                cmd = new OleDbCommand();
            }

            DataTable dt = new DataTable();
            OleDbConnection con = (OleDbConnection)get_connection("access");
            con.Open();
            cmd.CommandText = sql;
            cmd.Connection = con;
            OleDbDataAdapter sda = new OleDbDataAdapter(cmd);
            sda.Fill(dt);
            con.Close();
            return dt;
        }
        public static DataTable select(string sql, SqlCommand cmd = null)
        {
            if (cmd == null)
            {
                cmd = new SqlCommand();
            }

            DataTable dt = new DataTable();
            SqlConnection con = (SqlConnection)get_connection("sql");
            con.Open();
            cmd.CommandText = sql;
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            con.Close();
            return dt;
        }
        public static string serializeDataTable(string sql, OleDbCommand cmd = null)
        {
            DataTable dt = select(sql, cmd);
            return JsonConvert.SerializeObject(dt);
        }
        public static string serializeDataTable(string sql, SqlCommand cmd = null)
        {
            DataTable dt = select(sql, cmd);
            return JsonConvert.SerializeObject(dt);
        }
        public static string create_update_delete(string sql, SqlCommand cmd = null)
        {
            if (cmd == null)
            {
                cmd = new SqlCommand();
            }

            SqlConnection con = (SqlConnection)get_connection("sql");
            cmd.CommandText = sql;
            cmd.Connection = con;
            con.Open();
            int rows_effected = cmd.ExecuteNonQuery();
            con.Close();
            return rows_effected.ToString();
        }
        public static string create_update_delete(string sql, OleDbCommand cmd = null)
        {
            if (cmd == null)
            {
                cmd = new OleDbCommand();
            }

            OleDbConnection con = (OleDbConnection)get_connection("access");
            cmd.CommandText = sql;
            cmd.Connection = con;
            con.Open();
            int rows_effected = cmd.ExecuteNonQuery();
            con.Close();
            return rows_effected.ToString();
        }
    }
}