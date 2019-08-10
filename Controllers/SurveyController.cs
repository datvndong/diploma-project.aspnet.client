using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class SurveyController : BaseController {
        private readonly ISurveyService _surveyService;

        public SurveyController(IBaseService baseService, ISurveyService surveyService) : base(baseService) {
            _surveyService = surveyService;
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file) {
            string adminAuthenResult = await AdminAuthentication();
            if (!adminAuthenResult.Equals(string.Empty)) {
                return View(adminAuthenResult);
            }

            User user = GetUser();
            string importer = user.Email;

            try {
                if (file.ContentLength > 0) {
                    string fileName = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(fileName);
                    bool insertResult = false;

                    if (extension.Equals("." + Keywords.XLSX)) {
                        string path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                        file.SaveAs(path);

                        List<string> surveys = _surveyService.GetListDataFromFile(path, fileName, importer);
                        foreach (string survey in surveys) {
                            insertResult = _surveyService.Insert(survey);
                            if (!insertResult) {
                                TempData[Keywords.IMPORT] = Messages.IMPORT_FAILED;
                                return RedirectToAction(Keywords.INDEX, Keywords.FORM, new { page = 1 });
                            }
                        }

                        TempData[Keywords.IMPORT] = Messages.IMPORT_SUCCESSFUL;
                    } else {
                        TempData[Keywords.IMPORT] = Messages.IMPORT_FAILED;
                    }
                } else {
                    TempData[Keywords.IMPORT] = Messages.IMPORT_FAILED;
                }
            } catch {
                TempData[Keywords.IMPORT] = Messages.IMPORT_FAILED;
            }

            return RedirectToAction(Keywords.INDEX, Keywords.FORM,new { page = 1 });
        }
    }
}