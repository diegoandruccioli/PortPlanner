using Microsoft.AspNetCore.Mvc;
using PortPlanner.Domain;
using PortPlanner.Web.Areas;

namespace PortPlanner.Web.Features.Dashboard
{
    // Destinazione del login: manda ogni profilo alla sua home. Il Dipendente resta sulla pagina
    // provvisoria finché non c'è l'app mobile.
    public partial class DashboardController : AuthenticatedBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            if (Identita.Profilo == Profilo.Dipendente)
                return View();

            return RedirectToAction("Index", "Home", new { area = Identita.Profilo.ToString() });
        }
    }
}
