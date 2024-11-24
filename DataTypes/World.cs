using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1980scape.DataTypes {
    public class World {
        [JsonIgnore]
        public Dictionary<string, Dictionary<Point, Map>> Atlas = new(); 


        public SpecificPosition LastGoodPos = new(5, 5, 0, 0, "Overworld");


        public Dictionary<TwoWayString, Recipe> UseRecipes = new();
        public Dictionary<string, Item> ItemDatabase = new();

        public Map? TryGetMap(SpecificPosition where) { 
            if (Atlas.ContainsKey(where.WorldArea)) { 
                if (Atlas[where.WorldArea].ContainsKey(where.GetMapPos())) {
                    return Atlas[where.WorldArea][where.GetMapPos()];
                } 
            }

            return null;
        }

        public void AddMap(SpecificPosition where) {
            if (Atlas.ContainsKey(where.WorldArea)) {  
                if (!Atlas[where.WorldArea].ContainsKey(where.GetMapPos()))
                    Atlas[where.WorldArea].Add(where.GetMapPos(), new());
            }
        }
    }
}
