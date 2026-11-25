namespace PortPlanner.Domain
{
    // Chi ha deciso che un team lavora in quello slot.
    public enum OrigineAssegnazione
    {
        // Imposto dal supervisore: prevale sempre e il capogruppo non lo contesta.
        Supervisore = 0,

        // Scelto dal capogruppo del team che gestisce la barca.
        Capogruppo = 1
    }
}
