using Microsoft.AspNetCore.Mvc;

namespace PortPlanner.Web.Areas.Capogruppo
{
    public partial class TeamController : CapogruppoBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
