using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class LogManager
    {
        public string file_path { get; set; }

        public void InsertLog(string log_message)
        {
            try
            {
                File.AppendAllText(file_path, DateTime.Now.ToString() + ": " + log_message + Environment.NewLine);
            }
            catch (Exception ex)
            {
            }
        }
    }
}