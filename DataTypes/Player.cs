using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1980scape.DataTypes {
    public class Player {
        public SpecificPosition Position = new(5, 5, 0, 0, "Overworld");

        public int skinR = 221;
        public int skinG = 168;
        public int skinB = 160;

        public List<Item> Inventory = new();
        public Dictionary<string, Item> Equipment = new();

        public Dictionary<string, Skill> Skills = new();

        public ColoredString GetAppearance() {
            return new ColoredString("@", new Color(skinR, skinG, skinB), Color.Black);
        } 

        public Point GetPos() { return Position.GetPos(); } 
        public Point GetMapPos() { return Position.GetMapPos(); }
    }
}
