using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortPlanner.Web.Infrastructure;

namespace PortPlanner.Web.Areas.Supervisore
{
    // Base di tutte le pagine del Supervisore: chi ha un altro profilo viene respinto.
    [Area("Supervisore")]
    [Authorize(Roles = Profili.Supervisore)]
    public partial class SupervisoreBaseController : AuthenticatedBaseController
    {
    }
}
