﻿using _1980scape.UI;
using SadRogue.Primitives; 

namespace _1980scape.DataTypes {
    public class Map {
        public string MapName; 
        public Dictionary<Point, string> Tiles;
        public string DefaultTile;

        public Map() {
            MapName = "";
            Tiles = new();
            DefaultTile = "";
        } 
         

        public bool IsWalkable(int x, int y) {
            if (Tiles.ContainsKey(new Point(x, y))) { 
                if (GameLoop.UIManager.lofiHollow.TileLibrary.ContainsKey(Tiles[new Point(x, y)])) {
                    return GameLoop.UIManager.lofiHollow.TileLibrary[Tiles[new Point(x, y)]].Walkable;
                } 
            }

            if (DefaultTile != null) { 
                if (GameLoop.UIManager.lofiHollow.TileLibrary.ContainsKey(DefaultTile)) {
                    return GameLoop.UIManager.lofiHollow.TileLibrary[DefaultTile].Walkable;
                } 
            }

            return false;
        }
    }
}
