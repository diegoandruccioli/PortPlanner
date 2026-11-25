using PortPlanner.Domain;

namespace PortPlanner.Tests
{
    public class SlotTests
    {
        [Theory]
        [InlineData(Slot.Dalle0Alle6, 0, 6)]
        [InlineData(Slot.Dalle6Alle12, 6, 12)]
        [InlineData(Slot.Dalle12Alle18, 12, 18)]
        [InlineData(Slot.Dalle18Alle24, 18, 24)]
        public void Ogni_slot_ha_inizio_e_fine_giusti(Slot slot, int inizio, int fine)
        {
            Assert.Equal(inizio, slot.OraInizio());
            Assert.Equal(fine, slot.OraFine());
        }

        [Fact]
        public void Gli_slot_coprono_la_giornata_senza_buchi_e_sono_da_sei_ore()
        {
            var slot = Enum.GetValues<Slot>().OrderBy(s => s.OraInizio()).ToList();

            Assert.Equal(4, slot.Count);
            Assert.Equal(0, slot.First().OraInizio());
            Assert.Equal(24, slot.Last().OraFine());
            Assert.All(slot, s => Assert.Equal(SlotExtensions.OreDiUnoSlot, s.OraFine() - s.OraInizio()));

            for (var i = 1; i < slot.Count; i++)
                Assert.Equal(slot[i - 1].OraFine(), slot[i].OraInizio());
        }
    }
}
