using SadConsole; 

namespace _1980scape.DataTypes {
    public class LogEntry {
        public ColoredString Message = new("");
        public int Count = 1;

        public LogEntry(ColoredString msg) {
            Message = msg;
        }
    }
}
