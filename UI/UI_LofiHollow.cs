using SadConsole;
using SadRogue.Primitives;
using System;
using System.Linq; 
using ElectricDreams;
using Console = SadConsole.Console;
using Key = SadConsole.Input.Keys;
using ElectricDreams.Dreams.LofiHollow;
using SadConsole.Input;
using Newtonsoft.Json;

namespace ElectricDreams.UI {
    public class UI_LofiHollow : InstantUI {
        public LHWorld World;
        public LHPlayer Player;

        public Console Sidebar;
        public Console Minimap;

        public Console PaintingDisp;
        public Console DefaultTileDisp;
        public Console GlyphCon;

        public Dictionary<string, LHSkill> Skills = new();

        public string DisplayingTab = "Inventory";
        public string TypingField = "";
        public LHTile PaintingTile = new();
        public string PaintString = "";
        public Point LastTileChanged = new Point(-1, -1);

        public Dictionary<string, LHTile> TileLibrary = new();

        public List<LHLogEntry> MessageLog = new();

        public UI_LofiHollow(int width, int height, string winTitle = "") : base(width, height, "LofiHollow", winTitle) {  
            Con = new SadConsole.Console(new CellSurface(47, 47), GameLoop.SquareFont);
            Con.Font = GameLoop.SquareFont;

            Sidebar = new Console(68, 50);
            Sidebar.Position = new Point(82, 0);
            Win.Children.Add(Sidebar);

            Minimap = new Console(new CellSurface(9, 9), GameLoop.SquareFont);
            Minimap.Position = new Point(78, 1);
            Win.Children.Add(Minimap);

            PaintingDisp = new Console(new CellSurface(1, 1), GameLoop.SquareFont);
            PaintingDisp.Position = new Point(58, 17);
            Win.Children.Add(PaintingDisp);
             
            DefaultTileDisp = new Console(new CellSurface(1, 1), GameLoop.SquareFont);
            DefaultTileDisp.Position = new Point(82, 15);
            Win.Children.Add(DefaultTileDisp);


            GlyphCon = new Console(new CellSurface(32, 20), GameLoop.SquareFont);
            GlyphCon.Position = new Point(49, 23);
            Win.Children.Add(GlyphCon);


            Win.CanDrag = false;
            Helper.DrawBox(Win, 0, 0, width - 2, height - 2); 

            Con.UsePixelPositioning = true;

            Con.Position = new Point(9, 24);
            Win.Title = "".Align(HorizontalAlignment.Center, width - 2, (char)196);
            Win.Position = new Point(0, 0);


            Win.Children.Add(Con);
            GameLoop.UIManager.Children.Add(Win);

            Win.Show();
            //Win.IsVisible = false;

             
            InitWorld();
            InitSkills();
            InitLibraries();
        } 

        public override void Update() {
            MapDraw();
            SidebarDraw();
            MinimapDraw();
        }

        

        public void SidebarDraw() {
            Sidebar.Clear();
            Helper.DrawBox(Sidebar, 0, 0, Sidebar.Width, Sidebar.Height - 2);

            // Minimap Divider 
            Sidebar.DrawLine(new Point(51, 1), new Point(51, 9), 179, Color.White); 

            // Tab Choice Divider
            Sidebar.DrawLine(new Point(1, 10), new Point(66, 10), 196, Color.White);

            Sidebar.PrintClickable(2, 11, new ColoredString("Skills", DisplayingTab == "Skills" ? Color.Yellow : Color.White, Color.Black), (string a) => { DisplayingTab = a; }, "Skills");
            Sidebar.Print(9, 11, "|");
            Sidebar.PrintClickable(11, 11, new ColoredString("Inventory", DisplayingTab == "Inventory" ? Color.Yellow : Color.White, Color.Black), (string a) => { DisplayingTab = a; }, "Inventory");
            Sidebar.Print(21, 11, "|");
            Sidebar.PrintClickable(23, 11, new ColoredString("Equipment", DisplayingTab == "Equipment" ? Color.Yellow : Color.White, Color.Black), (string a) => { DisplayingTab = a; }, "Equipment");
            Sidebar.Print(33, 11, "|");

            Sidebar.PrintClickable(2, 9, new ColoredString("Map", DisplayingTab == "Dev Map" ? Color.Yellow : Color.White, Color.Black), () => { DisplayingTab = "Dev Map"; });
            Sidebar.Print(6, 9, "|");
            Sidebar.PrintClickable(8, 9, new ColoredString("Tile", DisplayingTab == "Dev Tile" ? Color.Yellow : Color.White, Color.Black), () => { DisplayingTab = "Dev Tile"; });
            Sidebar.Print(13, 9, "|");

            // Tab Content Divider
            Sidebar.DrawLine(new Point(1, 12), new Point(66, 12), 196, Color.White);

            if (DisplayingTab == "Skills") {
                if (Player != null) {
                    if (Player.Skills == null)
                        Player.Skills = new();

                    int line = 0;
                    foreach (KeyValuePair<string, LHSkill> kv in Player.Skills) {
                        Sidebar.Print(1, 13 + (2 * line), kv.Value.Name);
                        Sidebar.Print(15, 13 + (2 * line), "|", Color.DarkSlateGray);
                        Sidebar.Print(17, 13 + (2 * line), "Lv" + kv.Value.Level);
                        Sidebar.Print(22, 13 + (2 * line), "|", Color.DarkSlateGray); 
                        Sidebar.Print(24, 13 + (2 * line), ("Exp " + kv.Value.Exp.ToString("N0")).Align(HorizontalAlignment.Left, 15)); 
                        Sidebar.Print(39, 13 + (2 * line), "|", Color.DarkSlateGray);
                        Sidebar.Print(41, 13 + (2 * line), "Next " + kv.Value.EXPNeeded().ToString("N0")); 


                        Sidebar.DrawLine(new Point(1, 14 + (2 * line)), new Point(66, 14 + (2 * line)), '-', Color.DarkSlateGray);
                         


                        line++;
                    }
                }
            } else if (DisplayingTab == "Inventory") {
                Sidebar.Print(1, 13, "Inventory");
            } else if (DisplayingTab == "Equipment") {
                Sidebar.Print(1, 13, "Equipment");
            }

            if (DisplayingTab == "Dev Map") {

                if (World != null && Player != null) {
                    LHMap? current = World.TryGetMap(Player.Position);

                    if (current != null) {
                        Sidebar.PrintStringField(1, 13, "Map Name: ", ref current.MapName, ref TypingField, "mapName");

                        Sidebar.Print(1, 20, "Tile Library:");
                        int line = 21;

                        string delete = "";
                        foreach (var kv in TileLibrary) {
                            Sidebar.Print(1, line, "|");
                            Sidebar.PrintClickable(3, line, new ColoredString("*", Color.Crimson, Color.Black), () => { current.DefaultTile = kv.Key; });
                            Sidebar.PrintClickable(5, line, new ColoredString(kv.Key, PaintString == kv.Key ? Color.Green : Color.White, Color.Black), () => { 
                                PaintString = kv.Key;

                                if (TileLibrary.ContainsKey(PaintString))
                                    PaintingTile = Helper.Clone(TileLibrary[PaintString]);
                            });
                            line++;
                        } 

                        if (delete != "" && TileLibrary.ContainsKey(delete)) {
                            TileLibrary.Remove(delete);
                            delete = "";
                        }
                    }
                }
            }

            if (DisplayingTab == "Dev Tile") {
                PaintingDisp.IsVisible = true;
                GlyphCon.IsVisible = true;
                DefaultTileDisp.IsVisible = true;
                if (World != null && Player != null) {
                    LHMap? current = World.TryGetMap(Player.Position);

                    if (current != null) {
                        if (PaintingTile.Name == null)
                            PaintingTile.Name = "";
                        if (PaintingTile.ExamineText == null)
                            PaintingTile.ExamineText = "";

                        Sidebar.PrintStringField(1, 13, "   Tile Name: ", ref PaintingTile.Name, ref TypingField, "tileName"); 
                        Sidebar.PrintStringField(1, 14, "Examine Text: ", ref PaintingTile.ExamineText, ref TypingField, "tileExamine");

                        Sidebar.PrintScrollableInteger(1, 17, "Painting Glyph: ", ref PaintingTile.glyph, onlyPreface: true);
                        PaintingDisp.Print(0, 0, PaintingTile.GetAppearance());

                        for (int i = 0; i < 480; i++) { 
                            GlyphCon.PrintClickable((i % 32), (i / 32), new ColoredString(i.AsString(), PaintingTile.glyph == i ? Color.Green : Color.White, Color.Black), (string a) => { PaintingTile.glyph = i; }, "");
                        }

                        Sidebar.Print(1, 18, "Fore R: ");
                        Sidebar.PrintAdjustableInt(9, 18, 3, ref PaintingTile.colR, 0, 255);
                        Sidebar.Print(1, 19, "Fore G: ");
                        Sidebar.PrintAdjustableInt(9, 19, 3, ref PaintingTile.colG, 0, 255);
                        Sidebar.Print(1, 20, "Fore B: ");
                        Sidebar.PrintAdjustableInt(9, 20, 3, ref PaintingTile.colB, 0, 255);
                        Sidebar.PrintClickableBool(1, 21, "Walkable: ", ref PaintingTile.Walkable);
                         

                        Sidebar.Print(1, 1, current.Tiles.Count.ToString());
                        Sidebar.PrintClickable(62, 39, "SAVE", (string a) => { SaveCurrentWorldAsDefault(); }, "");

                        if (PaintingTile.Name != null && !TileLibrary.ContainsKey(PaintingTile.Name)) {
                            Sidebar.PrintClickable(1, 39, "ADD TO LIBRARY", () => { TileLibrary.Add(PaintingTile.Name, PaintingTile); });
                        } else if (PaintingTile.Name != null && TileLibrary.ContainsKey(PaintingTile.Name)) {
                            Sidebar.PrintClickable(1, 39, "UPDATE IN LIBRARY", () => { TileLibrary[PaintingTile.Name] = Helper.Clone(PaintingTile); });
                        } 
                    } else {
                        Sidebar.PrintClickable(1, 15, "Add a map here", () => { World.AddMap(Player.Position); });
                    }
                }
            } else {
                PaintingDisp.IsVisible = false;
                DefaultTileDisp.IsVisible = false;
                GlyphCon.IsVisible = false;
            }


            // Message Log Divider
            Sidebar.DrawLine(new Point(1, 40), new Point(66, 40), 196, Color.White);

            if (MessageLog.Count > 0) {
                for (int i = MessageLog.Count - 1; i >= 0 && i > MessageLog.Count - 9; i--) {
                    if (MessageLog[i].Count == 1) {
                        Sidebar.Print(1, 41 + (MessageLog.Count - 1 - i), MessageLog[i].Message);
                    } else { 
                        Sidebar.Print(1, 41 + (MessageLog.Count - 1 - i), MessageLog[i].Message + " (" + MessageLog[i].Count.ToString() + "x)");
                    }
                }
            }
        }

        public void MinimapDraw() {
            for (int i = 0; i < 9; i++) {
                Minimap.Print(i, i, "X");
                Minimap.Print(i, 8 - i, "X");
            }
        }

        public void MapDraw() {
            Con.Clear();
            Helper.DrawBox(Con, 0, 0, Con.Width - 2, Con.Height - 2);
            Win.Print(1, 1, "The Void Between All".Align(HorizontalAlignment.Center, 80));
             
            MouseScreenObjectState mouse = new MouseScreenObjectState(Con, GameHost.Instance.Mouse);
            Point mousePos = mouse.CellPosition - new Point(1, 1);
            bool mouseOn = mouse.IsOnScreenObject;

            bool FoundMap = false;

            if (World != null && Player != null) {
                LHMap? current = World.TryGetMap(Player.Position);

                if (current != null) { 
                    FoundMap = true;
                    Win.Print(1, 1, current.MapName.Align(HorizontalAlignment.Center, 80));

                    if (mouseOn) {
                        if (TileAt(current, mousePos) is LHTile hover) { 
                            if (hover.GatherSpot == null) {
                                if (hover.ExamineText != "") {
                                    Con.Print(1, 0, "Examine " + hover.Name);
                                } else { 
                                    Con.Print(1, 0, hover.Name);
                                }
                            } else { 
                                if (Helper.EitherShift() && hover.ExamineText != "") {
                                    Con.Print(1, 0, "Examine " + hover.Name);
                                } else { 
                                    Con.Print(1, 0, hover.Name);
                                }
                            }
                        }
                        else if (ResolveTile(current.DefaultTile) is LHTile hoverDef) {
                            Con.Print(1, 0, hoverDef.Name);
                        }
                    }


                    if (current.DefaultTile != null && current.DefaultTile != "") {
                        if (TileLibrary.ContainsKey(current.DefaultTile)) {
                            LHTile def = Helper.Clone(TileLibrary[current.DefaultTile]);
                            Con.Fill(new Rectangle(1, 1, Con.Width - 2, Con.Height - 2), def.GetColor(), Color.Black, def.glyph);
                        }
                    } 

                    LHTile errorTile = new LHTile() { colR = 255, glyph = 'X', Name = "ERROR" };
                    foreach (var kv in current.Tiles) { 
                        if (TileLibrary.ContainsKey(kv.Value)) {
                            LHTile tile = TileLibrary[kv.Value];
                            Con.Print(kv.Key.X + 1, kv.Key.Y + 1, tile.GetAppearance());
                        }
                        //Con.Print(kv.Key.X + 1, kv.Key.Y + 1, kv.Value.GetAppearance());
                    }

                    if (DisplayingTab == "Dev Map" || DisplayingTab == "Dev Tile") { 
                        if (GameHost.Instance.Mouse.LeftButtonDown && mouseOn) {
                            if (LastTileChanged != mousePos) {
                                LastTileChanged = mousePos;
                                if (Helper.EitherControl()) {
                                    if (current.Tiles.ContainsKey(mousePos))
                                        current.Tiles.Remove(mousePos);
                                }
                                else {
                                    if (current.Tiles.ContainsKey(mousePos)) {
                                        current.Tiles[mousePos] = PaintString;
                                    } else {
                                        current.Tiles.Add(mousePos, PaintString);
                                    } 
                                }
                                AddMessage(mousePos.ToString());
                            }
                        }
                        

                        if (GameHost.Instance.Mouse.RightButtonDown && mouseOn) {
                            if (current.Tiles.ContainsKey(mousePos))
                                PaintString = current.Tiles[mousePos];
                        } 
                    } else {
                        if (mouseOn && GameHost.Instance.Mouse.LeftClicked) { 
                            if (TileAt(current, mousePos) is LHTile hover) { 
                                if (hover.GatherSpot == null) {
                                    if (hover.ExamineText != "") {
                                        AddMessage(hover.ExamineText);
                                    }
                                    else {
                                        Con.Print(1, 0, hover.Name);
                                    }
                                }
                                else {
                                    if (Helper.EitherShift() && hover.ExamineText != "") {
                                        AddMessage(hover.ExamineText);
                                    }
                                    else {
                                        Con.Print(1, 0, hover.Name);
                                    }
                                } 
                            }
                            else if (ResolveTile(current.DefaultTile) is LHTile hoverDef) {
                                if (hoverDef.GatherSpot == null) {
                                    if (hoverDef.ExamineText != "") {
                                        AddMessage(hoverDef.ExamineText);
                                    }
                                    else {
                                        Con.Print(1, 0, hoverDef.Name);
                                    }
                                }
                                else {
                                    if (Helper.EitherShift() && hoverDef.ExamineText != "") {
                                        AddMessage(hoverDef.ExamineText);
                                    }
                                    else {
                                        Con.Print(1, 0, hoverDef.Name);
                                    }
                                }
                            }
                        }
                    }
                }

                if (FoundMap) {
                    Con.Print(Player.Position.X + 1, Player.Position.Y + 1, Player.GetAppearance()); 
                }
            } 


            if (!FoundMap) {
                Con.Print(1, 20, "you are somewhere you should not be".Align(HorizontalAlignment.Center, 45)); 
                Con.PrintClickable(1, 22, "click here to return".Align(HorizontalAlignment.Center, 45), UI_Clicks, "VoidReturn");
            }
        }

        public override void Input() { 

            if (Helper.KeyPressed(Key.W) || Helper.KeyPressed(Key.Up) || Helper.KeyPressed(Key.NumPad8)) {
                TryMove(0, -1);
            }

            if (Helper.KeyPressed(Key.S) || Helper.KeyPressed(Key.Down) || Helper.KeyPressed(Key.NumPad2)) {
                TryMove(0, 1);
            }

            if (Helper.KeyPressed(Key.A) || Helper.KeyPressed(Key.Left) || Helper.KeyPressed(Key.NumPad4)) {
                TryMove(-1, 0);
            }

            if (Helper.KeyPressed(Key.D) || Helper.KeyPressed(Key.Right) || Helper.KeyPressed(Key.NumPad6)) {
                TryMove(1, 0);
            }


            if (DisplayingTab == "Dev Map") {
                if (World != null && Player != null) {
                    LHMap? current = World.TryGetMap(Player.Position);

                    if (current != null) {
                        Dictionary<Point, string> temp = new();
                        if (Helper.EitherShift() && Helper.KeyPressed(Key.W)) {
                            foreach (var kv in current.Tiles) {
                                temp.Add(kv.Key + new Point(0, -1), kv.Value);
                            }

                            current.Tiles = Helper.Clone(temp);
                        } else if (Helper.EitherShift() && Helper.KeyPressed(Key.S)) {
                            foreach (var kv in current.Tiles) {
                                temp.Add(kv.Key + new Point(0, 1), kv.Value);
                            }

                            current.Tiles = Helper.Clone(temp);
                        } else if (Helper.EitherShift() && Helper.KeyPressed(Key.A)) {
                            foreach (var kv in current.Tiles) {
                                temp.Add(kv.Key + new Point(-1, 0), kv.Value);
                            }

                            current.Tiles = Helper.Clone(temp);
                        } else if (Helper.EitherShift() && Helper.KeyPressed(Key.D)) {
                            foreach (var kv in current.Tiles) {
                                temp.Add(kv.Key + new Point(1, 0), kv.Value);
                            }

                            current.Tiles = Helper.Clone(temp);
                        }

                        if (Helper.HotkeyDown(Key.U)) { 
                        }
                    }
                }
            }
        }

        public override void UI_Clicks(string ID) {
            if (ID == "VoidReturn") {
                if (World != null && Player != null) {
                    Player.Position = Helper.Clone(World.LastGoodPos);
                }
            }
        }


        public void InitWorld() {
            World = new();
            Player = new();

            LoadDefaultWorld();
            //LoadPlayer();
        }

        public void InitSkills() {
            Skills.Add("Attack", new("Attack"));
            Skills.Add("Strength", new("Strength"));
            Skills.Add("Defense", new("Defense"));
            Skills.Add("Constitution", new("Constitution"));
            Skills.Add("Magic", new("Magic"));
            Skills.Add("Mining", new("Mining"));
            Skills.Add("Smithing", new("Smithing"));
            Skills.Add("Farming", new("Farming")); 
            Skills.Add("Cooking", new("Cooking"));
            Skills.Add("Herblore", new("Herblore"));
            Skills.Add("Crafting", new("Crafting"));
            Skills.Add("Runecrafting", new("Runecrafting"));

            foreach (KeyValuePair<string, LHSkill> skill in Skills) {
                if (!Player.Skills.ContainsKey(skill.Key)) {
                    Player.Skills.Add(skill.Key, Helper.Clone(skill.Value));
                }
            }
        }

        public void InitLibraries() {
            if (Directory.Exists("./data/tiles/")) {
                string[] tileFiles = Directory.GetFiles("./data/tiles/");

                foreach (string fileName in tileFiles) {
                    string json = File.ReadAllText(fileName);
                    LHTile item = JsonConvert.DeserializeObject<LHTile>(json);
                    TileLibrary.Add(item.Name, item);
                }
            }
        }

        public void SaveCurrentWorldAsDefault() {
            if (File.Exists("./data/World.json")) {
                if (File.Exists("./data/World-bk.json")) {
                    File.Delete("./data/World-bk.json");
                }
                File.Copy("./data/World.json", "./data/World-bk.json");
            } 

            foreach (var kv in TileLibrary) {
                Helper.SerializeToFile(kv.Value, "./data/tiles/" + kv.Key + ".json");
            }

            foreach (var kv in World.Overworld) {
                Helper.SerializeToFileCompressed(kv.Value, "./data/maps/Overworld/" + kv.Key + ".json");
            }

           //Helper.SerializeToFile(World, "./data/lofihollow/lhWorld.json");
        }

        public void LoadDefaultWorld() {
            if (File.Exists("./data/World.json")) {
                World = Helper.DeserializeFromFile<LHWorld>("./data/World.json"); 
            }
        }

        public void AddMessage(string msg) {
            AddMessage(new ColoredString(msg));
        }

        public void AddMessage(ColoredString msg) {
            if (MessageLog.Count > 0) {
                if (MessageLog[^1].Message.String == msg.String) {
                    MessageLog[^1].Count++;
                } else {
                    MessageLog.Add(new(msg));
                }
            } else {
                MessageLog.Add(new(msg));
            }
        }

        public LHTile? TileAt(LHMap map, Point pos) {
            if (map.Tiles.ContainsKey(pos)) {
                return ResolveTile(map.Tiles[pos]);
            }
            return null;
        }

        public LHTile? ResolveTile(string name) {
            if (name != null) {
                if (TileLibrary.ContainsKey(name))
                    return TileLibrary[name];
            }
            return null;
        }

        public void TryMove(int dx, int dy) {
            if (World != null && Player != null) {
                LHMap? current = World.TryGetMap(Player.Position);

                if (current != null) {
                    int nx = Player.Position.X + dx;
                    int ny = Player.Position.Y + dy;

                    if (nx >= 0 && nx <= 45 && ny >= 0 && ny <= 45) {
                        if (current.IsWalkable(nx, ny)) {
                            Player.Position.X += dx;
                            Player.Position.Y += dy;
                        }
                    }
                } 
            }
        }
    }
}
