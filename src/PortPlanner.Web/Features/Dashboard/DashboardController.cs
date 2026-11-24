using Microsoft.AspNetCore.Mvc;
using PortPlanner.Web.Areas;

namespace PortPlanner.Web.Features.Dashboard
{
    /// <summary>
    /// Pagina iniziale provvisoria dopo il login.
    /// Verrà sostituita dalle home dei tre profili (supervisore, capogruppo, dipendente).
    /// </summary>
    public partial class DashboardController : AuthenticatedBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}
