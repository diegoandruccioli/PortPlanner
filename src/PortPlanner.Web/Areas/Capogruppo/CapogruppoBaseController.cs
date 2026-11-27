using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortPlanner.Web.Infrastructure;

namespace PortPlanner.Web.Areas.Capogruppo
{
    // Base di tutte le pagine del Capogruppo: chi ha un altro profilo viene respinto.
    [Area("Capogruppo")]
    [Authorize(Roles = Profili.Capogruppo)]
    public partial class CapogruppoBaseController : AuthenticatedBaseController
    {
    }
}
