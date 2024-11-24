using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1980scape.DataTypes {
    public class Recipe {
        public string FirstItem = "";
        public int FirstQty = 1;
        public string SecondItem = "";
        public int SecondQty = 1;

        public string OutputItem = "";
        public int OutputQty = 1;

        public string SkillUsed = "";
        public int SkillLevelReq = 1;
        public int ExpGranted = 0;
    }
}
