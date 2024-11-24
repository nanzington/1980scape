using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricDreams.Dreams.LofiHollow {
    public class LHWorld {
        public Dictionary<Point, LHMap> Overworld = new(); 
        public SpecificPosition LastGoodPos = new(5, 5, 0, 0, "Overworld");


        public Dictionary<TwoWayString, LHRecipe> UseRecipes = new();
        public Dictionary<string, LHItem> ItemDatabase = new();

        public LHMap? TryGetMap(SpecificPosition where) {
            if (where.WorldArea == "Overworld") { 
                if (Overworld.ContainsKey(where.GetMapPos())) {
                    return Overworld[where.GetMapPos()];
                } 
            }

            return null;
        }

        public void AddMap(SpecificPosition where) {
            if (where.WorldArea == "Overworld") {  
                if (!Overworld.ContainsKey(where.GetMapPos()))
                    Overworld.Add(where.GetMapPos(), new());
            }
        }
    }
}
