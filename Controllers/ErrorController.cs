using CentralizedDataSystem.Resources;
using System.Web.Mvc;

namespace CentralizedDataSystem.Controllers {
    public class ErrorController : Controller {
        
        public ActionResult Index() {
            return View(ViewName.ERROR_UNKNOWN);
        }

        public ViewResult NotFound() {
            Response.StatusCode = 404;

            return View(ViewName.ERROR_404);
        }
    }
}