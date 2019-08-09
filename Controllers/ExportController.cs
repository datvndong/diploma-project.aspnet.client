using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class ExportController : BaseController {
        private readonly IExportService _exportService;

        public ExportController(IBaseService baseService, IExportService exportService) : base(baseService) {
            _exportService = exportService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string path, string type) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string token = user.Token;

            string submissionsData = await _exportService.ExportSubmissionsDataToString(token, path, type);

            var contentDisposition = new System.Net.Mime.ContentDisposition { FileName = path + "." + type, Inline = false };
            byte[] arr = Encoding.UTF8.GetBytes(submissionsData);

            string mediaType = Keywords.MEDIA_TYPE_JSON;
            if (type.Equals(Keywords.CSV)) {
                mediaType = Keywords.MEDIA_TYPE_CSV;
            }

            Response.ContentType = mediaType;
            Response.AddHeader("content-disposition", contentDisposition.ToString());
            Response.ContentEncoding = Encoding.UTF8;
            Response.Buffer = true;
            Response.Clear();
            Response.BinaryWrite(arr);
            Response.End();

            return new FileStreamResult(Response.OutputStream, Response.ContentType);
        }
    }
}