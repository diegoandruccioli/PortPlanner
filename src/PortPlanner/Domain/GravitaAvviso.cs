namespace PortPlanner.Domain
{
    // Un avviso non blocca mai l'assegnazione: la gravità decide solo quanto in fondo finisce il candidato.
    public enum GravitaAvviso
    {
        Normale = 0,

        // Riservata a ferie, malattia e contratto scaduto: le regole di riposo e di turni consecutivi sono sempre Normale.
        Forte = 1
    }
}
