using Microsoft.AspNetCore.Mvc;

namespace PortPlanner.Web.Areas.Supervisore
{
    public partial class AssenzeController : SupervisoreBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
