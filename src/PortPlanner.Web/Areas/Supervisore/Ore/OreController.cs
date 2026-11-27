using Microsoft.AspNetCore.Mvc;

namespace PortPlanner.Web.Areas.Supervisore
{
    public partial class OreController : SupervisoreBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
