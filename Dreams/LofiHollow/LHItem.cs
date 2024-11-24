using SadConsole;
using SadRogue.Primitives;

namespace ElectricDreams.Dreams.LofiHollow {
    public class LHItem {
        public string Name = "";
        public string ExamineText = "";
        public string ID = "";

        public int colR = 255;
        public int colG = 255;
        public int colB = 255;
        public int glyph = 1;

        public int Quantity = 1;
        public bool Stackable = false;

        public string EquipSlot = "";

        public ColoredString GetAppearance() {
            return new ColoredString(glyph.AsString(), new Color(colR, colG, colB), Color.Black);
        }
    }
}
