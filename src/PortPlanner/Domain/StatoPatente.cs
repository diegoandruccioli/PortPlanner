namespace PortPlanner.Domain
{
    // Non si salva: si calcola ogni volta dalla scadenza della patente.
    public enum StatoPatente
    {
        Valida = 0,

        // Scade entro 30 giorni (vedi Patente.GiorniPreavvisoScadenza).
        InScadenza = 1,

        Scaduta = 2
    }
}
