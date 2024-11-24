using SadConsole; 

namespace ElectricDreams.Dreams.LofiHollow {
    public class LHLogEntry {
        public ColoredString Message = new("");
        public int Count = 1;

        public LHLogEntry(ColoredString msg) {
            Message = msg;
        }
    }
}
