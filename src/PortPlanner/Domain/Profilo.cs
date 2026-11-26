namespace PortPlanner.Domain
{
    // Il profilo non è un campo del database: si calcola al login e finisce nel cookie.
    // Decide la home e le pagine a cui si può accedere.
    public enum Profilo
    {
        // Utente con IsSupervisore. Non è un lavoratore: niente turni, niente team.
        Supervisore = 0,

        // Lavoratore che è il capogruppo di un team (Team.CapogruppoId). È un incarico, non un ruolo.
        Capogruppo = 1,

        // Tutti gli altri lavoratori, con o senza team.
        Dipendente = 2
    }
}
