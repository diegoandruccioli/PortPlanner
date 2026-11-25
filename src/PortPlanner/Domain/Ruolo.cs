namespace PortPlanner.Domain
{
    // Il capogruppo non è un ruolo: è un incarico di un team.
    public enum Ruolo
    {
        Gruista = 0,
        Muletista = 1,

        // Non serve la patente e lo può fare chiunque.
        Operaio = 2
    }
}
