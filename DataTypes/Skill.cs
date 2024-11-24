using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1980scape.DataTypes {
    public class Skill {
        public string Name = "";
        public int Level = 1;
        public int Exp = 0;

        public Skill(string n) {
            Name = n;
        }

        public int ExpToLevel() { 
            return (int)(Math.Floor(0.125 * (Level + 1) * (Level)) 
                       + Math.Floor(75 * ((Math.Pow(2, (Level + 1) / 7f) - Math.Pow(2, 1f / 7f)) / (Math.Pow(2, 1f / 7f) - 1)))
                       + Math.Floor(0.109 * (Level + 1)));
        }

        public int EXPNeeded() {
            return ExpToLevel() - Exp;
        }
    }
}
