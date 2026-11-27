using Microsoft.AspNetCore.Mvc;

namespace PortPlanner.Web.Areas.Supervisore
{
    public partial class RichiesteController : SupervisoreBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
