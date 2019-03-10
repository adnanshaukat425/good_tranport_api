using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace web_api_for_good_transport.Models
{
    public class CustomUploadMultipartFormProvider : MultipartFormDataStreamProvider
    {
        public CustomUploadMultipartFormProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers != null && headers.ContentDisposition != null)
            {
                string log_file_path = HttpContext.Current.Server.MapPath("~/Logs/UploadLog.txt");
                File.AppendAllText(log_file_path, "---FILE_NAME " + headers
                    .ContentDisposition
                    .FileName.TrimEnd('"').TrimStart('"') + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);

                return headers
                    .ContentDisposition
                    .FileName.TrimEnd('"').TrimStart('"');

                
            }

            return base.GetLocalFileName(headers);
        }
    }
}