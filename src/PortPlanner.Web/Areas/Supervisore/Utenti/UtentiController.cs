using Microsoft.AspNetCore.Mvc;

namespace PortPlanner.Web.Areas.Supervisore
{
    public partial class UtentiController : SupervisoreBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
