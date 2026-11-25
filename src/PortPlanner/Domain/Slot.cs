namespace PortPlanner.Domain
{
    // Le quattro fasce della giornata, ognuna da 6 ore.
    public enum Slot
    {
        Dalle0Alle6 = 0,
        Dalle6Alle12 = 1,
        Dalle12Alle18 = 2,
        Dalle18Alle24 = 3
    }

    public static class SlotExtensions
    {
        public const int OreDiUnoSlot = 6;

        // Ora di inizio dello slot: 0, 6, 12 o 18.
        public static int OraInizio(this Slot slot) => (int)slot * OreDiUnoSlot;

        // Ora di fine dello slot: 6, 12, 18 o 24 (mezzanotte).
        public static int OraFine(this Slot slot) => slot.OraInizio() + OreDiUnoSlot;
    }
}
