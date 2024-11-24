using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1980scape.DataTypes {
    public class GatheringTile {
        public string Skill = "";
        public int Level = 1;
        public int ExpGranted = 0;

        public List<WeightedItem> PossibleItems = new();
    }
}
