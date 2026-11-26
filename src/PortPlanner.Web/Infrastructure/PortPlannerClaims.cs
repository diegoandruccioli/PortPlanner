namespace PortPlanner.Web.Infrastructure
{
    // Nomi dei claim che mettiamo nel cookie al login, oltre a quelli standard
    // (NameIdentifier, Email, Name e Role, che contiene il profilo).
    public static class PortPlannerClaims
    {
        // Vuoto per il Supervisore, che non è un lavoratore.
        public const string DipendenteId = "PortPlanner.DipendenteId";

        // Presente solo per chi ha un team. Se cambia il capogruppo o il team, serve un nuovo accesso.
        public const string TeamId = "PortPlanner.TeamId";
    }
}
