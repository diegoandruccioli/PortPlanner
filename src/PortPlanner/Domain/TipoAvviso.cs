namespace PortPlanner.Domain
{
    // Il tipo di regola contrattuale a cui si riferisce un avviso. Le regole nuove si aggiungono in fondo.
    public enum TipoAvviso
    {
        // Meno di 11 ore tra la fine di un blocco di lavoro e l'inizio del successivo.
        RiposoGiornaliero = 0,

        // Due o più slot di fila (12 ore o più di lavoro continuo), anche a cavallo di mezzanotte.
        TurniConsecutivi = 1
    }
}
