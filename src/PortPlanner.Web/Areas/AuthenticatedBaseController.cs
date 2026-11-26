using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using PortPlanner.Domain;
using PortPlanner.Web.Infrastructure;

namespace PortPlanner.Web.Areas
{
    [Authorize]
    [Alerts]
    [ModelStateToTempData]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AuthenticatedBaseController : Controller
    {
        public AuthenticatedBaseController() { }

        protected IdentitaViewModel Identita
        {
            get
            {
                return (IdentitaViewModel)ViewData[IdentitaViewModel.VIEWDATA_IDENTITACORRENTE_KEY];
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var identita = LeggiIdentita(context);

                if (identita != null)
                {
                    ViewData[IdentitaViewModel.VIEWDATA_IDENTITACORRENTE_KEY] = identita;
                }
                else
                {
                    // Nessun accesso valido, oppure un cookie vecchio senza il profilo: si torna al login.
                    HttpContext.SignOutAsync();
                    this.SignOut();

                    context.Result = new RedirectResult(context.HttpContext.Request.GetEncodedUrl());
                    Alerts.AddError(this, "L'utente non possiede i diritti per visualizzare la risorsa richiesta");
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Ricostruisce l'identità dai claim del cookie. Se manca qualcosa d'essenziale restituisce null.
        private static IdentitaViewModel LeggiIdentita(ActionExecutingContext context)
        {
            var utente = context.HttpContext?.User;

            if (utente == null || utente.Identity.IsAuthenticated == false)
                return null;

            string Claim(string tipo) => utente.Claims.FirstOrDefault(x => x.Type == tipo)?.Value;

            var email = Claim(ClaimTypes.Email);
            var nome = Claim(ClaimTypes.Name);

            if (email == null || nome == null || Enum.TryParse<Profilo>(Claim(ClaimTypes.Role), out var profilo) == false)
                return null;

            return new IdentitaViewModel
            {
                EmailUtenteCorrente = email,
                NomeCompleto = nome,
                Profilo = profilo,
                DipendenteId = Guid.TryParse(Claim(PortPlannerClaims.DipendenteId), out var dipendenteId) ? dipendenteId : null,
                TeamId = Guid.TryParse(Claim(PortPlannerClaims.TeamId), out var teamId) ? teamId : null,
            };
        }
    }
}
