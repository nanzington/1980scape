﻿using SadConsole;
using SadRogue.Primitives; 

namespace ElectricDreams.Dreams.LofiHollow {
    public class LHTile {
        public string Name = "";
        public int colR;
        public int colG;
        public int colB;

        public int glyph;

        public bool Walkable = true;

        public GatheringTile GatherSpot;

        public string ExamineText = "";


        public ColoredString GetAppearance() {
            return new ColoredString(glyph.AsString(), new Color(colR, colG, colB, 255), Color.Black);
        }
        
        public Color GetColor() {
            return new Color(colR, colG, colB, 255);
        }
    }
}
