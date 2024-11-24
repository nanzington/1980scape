using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricDreams.Dreams.LofiHollow {
    public class LHPlayer {
        public SpecificPosition Position = new(5, 5, 0, 0, "Overworld");

        public int skinR = 221;
        public int skinG = 168;
        public int skinB = 160;

        public List<LHItem> Inventory = new();
        public Dictionary<string, LHItem> Equipment = new();

        public Dictionary<string, LHSkill> Skills = new();

        public ColoredString GetAppearance() {
            return new ColoredString("@", new Color(skinR, skinG, skinB), Color.Black);
        } 

        public Point GetPos() { return Position.GetPos(); } 
        public Point GetMapPos() { return Position.GetMapPos(); }
    }
}
