using PortPlanner.Domain;

namespace PortPlanner.Web.Infrastructure
{
    // Nomi dei profili da usare in [Authorize(Roles = ...)]: sono i valori che il login mette in ClaimTypes.Role.
    public static class Profili
    {
        public const string Supervisore = nameof(Profilo.Supervisore);

        public const string Capogruppo = nameof(Profilo.Capogruppo);

        public const string Dipendente = nameof(Profilo.Dipendente);
    }
}
