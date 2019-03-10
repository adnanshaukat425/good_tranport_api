using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using web_api_for_good_transport.Models;
namespace web_api_for_good_transport.Controllers
{
    public class UploadImageController : ApiController
    {
        [Route("api/upload")]
        public async Task<HttpResponseMessage> Post()
        {
            try
            {
                string log_file_path = HttpContext.Current.Server.MapPath("~/Logs/UploadLog.txt");
                File.AppendAllText(log_file_path, "---FileUploadingStarted---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
                if (!Request.Content.IsMimeMultipartContent())
                {
                    File.AppendAllText(log_file_path, "---NoFileFound---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);

                    File.AppendAllText(log_file_path, "---UnsupportedFormat---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine); throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var files = HttpContext.Current.Request.Files;
                var uploadPath = HttpContext.Current.Server.MapPath("~/Images/AppImages");
                var multipartFormDataStreamProvider = new CustomUploadMultipartFormProvider(uploadPath);

                File.AppendAllText(log_file_path, "---FileUploaded " + uploadPath + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);

                File.AppendAllText(log_file_path, "---FileUploaded " + uploadPath + "---" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);

                await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);
                File.AppendAllText(log_file_path, "---FileUploadedEnded" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage(HttpStatusCode.NotImplemented)
                {
                    Content = new StringContent(e.Message)
                };
            }
        }
    }
}