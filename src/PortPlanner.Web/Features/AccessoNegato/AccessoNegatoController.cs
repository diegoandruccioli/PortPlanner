using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortPlanner.Web.Areas;

namespace PortPlanner.Web.Features.AccessoNegato
{
    // Qui arriva chi apre una pagina di un altro profilo (AccessDeniedPath del cookie).
    // Deriva dal controller autenticato per mostrare il menu del proprio profilo.
    public partial class AccessoNegatoController : AuthenticatedBaseController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return View();
        }
    }
}
