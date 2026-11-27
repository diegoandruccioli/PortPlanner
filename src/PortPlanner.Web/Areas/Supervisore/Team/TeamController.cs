using Microsoft.AspNetCore.Mvc;

namespace PortPlanner.Web.Areas.Supervisore
{
    public partial class TeamController : SupervisoreBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
