using Newtonsoft.Json;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System.ComponentModel;
using System.Globalization;
using System.IO.Compression;
using System.Text; 
using System.Text.RegularExpressions;
using Console = SadConsole.Console;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace ElectricDreams {
    public static class Helper {
        public static bool ProcessedClick = false;
        public static double CursorTicked = 0;
        public static bool CursorVisible = true;

        public static double Time() {
            return Game.Instance.GameRunningTotalTime.TotalMilliseconds;
        }

        public static ColoredString Checkmark(bool condition) {
            ColoredString check = new ColoredString(256.AsString(), Color.Lime, Color.Black);

            if (!condition)
                check = new ColoredString(257.AsString(), Color.Red, Color.Black);

            return check;
        }

        public static string Truncate(string input, int len) {
            if (input.Length < len)
                return input;
            return input[0..(len - 1)];
        }


        public static bool KeyPressed(Keys key) {
            return GameHost.Instance.Keyboard.IsKeyPressed(key);
        }

        public static bool KeyDown(Keys key) {
            return GameHost.Instance.Keyboard.IsKeyDown(key);
        }

        static HashSet<Keys> TriggeredHotkeys = new();
        static HashSet<Keys> SecondaryList = new();
        public static bool HotkeyDown(Keys key) {
            if (!TriggeredHotkeys.Contains(key) && GameHost.Instance.Keyboard.IsKeyPressed(key)) {
                TriggeredHotkeys.Add(key);
                return true;
            }

            return false;
        }

        public static void ClearKeys() {
            SecondaryList.Clear();
            foreach (Keys key in TriggeredHotkeys) {
                if (GameHost.Instance.Keyboard.IsKeyDown(key)) {
                    SecondaryList.Add(key);
                }
            }
            TriggeredHotkeys.Clear();

            foreach (Keys key in SecondaryList) {
                TriggeredHotkeys.Add(key);
            }
        }

        public static bool EitherShift() {
            if (GameHost.Instance.Keyboard.IsKeyDown(Keys.LeftShift) || GameHost.Instance.Keyboard.IsKeyDown(Keys.RightShift))
                return true;
            return false;
        }
        public static bool EitherControl() {
            if (GameHost.Instance.Keyboard.IsKeyDown(Keys.LeftControl) || GameHost.Instance.Keyboard.IsKeyDown(Keys.RightControl))
                return true;
            return false;
        } 

        public static T Clone<T>(this T source) {
            if (Object.ReferenceEquals(source, null)) {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var serializeSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            TypeDescriptor.AddAttributes(typeof(Point), new TypeConverterAttribute(typeof(PointConverter)));

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
        } 

        public static void DrawBox(Console con, int LeftX, int TopY, int w, int h, int r = 255, int g = 255, int b = 255, int mode = 0) {
            int LeftXin = LeftX + 1;
            int TopYin = TopY + 1;
            int RightXin = LeftX + w;
            int BottomYin = TopY + h;
            int BottomY = TopY + h + 1;
            int RightX = LeftX + w + 1;
             
            Color fg = new Color(r, g, b);

            if (mode == 0) {
                con.DrawLine(new Point(LeftXin, TopY), new Point(RightXin, TopY), 320, fg);
                con.DrawLine(new Point(LeftXin, BottomY), new Point(RightXin, BottomY), 320, fg);
                con.DrawLine(new Point(LeftX, TopYin), new Point(LeftX, BottomYin), 321, fg);
                con.DrawLine(new Point(RightX, TopYin), new Point(RightX, BottomYin), 321, fg);
                con.Print(LeftX, BottomY, 322.AsString(), fg);
                con.Print(RightX, BottomY, 323.AsString(), fg);
                con.Print(LeftX, TopY, 324.AsString(), fg); 
                con.Print(RightX, TopY, 325.AsString(), fg);
            } else if (mode == 1) {
                con.DrawLine(new Point(LeftXin, TopY), new Point(RightXin, TopY), 190, fg);
                con.DrawLine(new Point(LeftXin, BottomY), new Point(RightXin, BottomY), 190, fg);
                con.DrawLine(new Point(LeftX, TopYin), new Point(LeftX, BottomYin), 191, fg);
                con.DrawLine(new Point(RightX, TopYin), new Point(RightX, BottomYin), 191, fg);
                con.Print(LeftX, TopY, 188.AsString(), fg);
                con.Print(RightX, BottomY, 187.AsString(), fg);
                con.Print(LeftX, BottomY, 186.AsString(), fg);
                con.Print(RightX, TopY, 189.AsString(), fg);
            }
        }  

        public static ColoredString ToColoredString(this List<Color> instance, string text) {
            ColoredString build = new();

            for (int i = 0; i < instance.Count && i < text.Length; i++) {
                build += new ColoredString(text.Substring(i, 1), instance[i], Color.Black);
            }

            return build;
        }

        public static string AsString(this int instance) {
            return ((char)instance).ToString();
        } 

        public static ColoredString GetDarker(this ColoredString instance) {

            for (int i = 0; i < instance.Length; i++) {
                instance[i].Foreground = instance[i].Foreground.GetDarker();
            }

            return instance;
        } 

        public static void Flip(this ref bool instance) {
            instance = !instance;
        }

        public static void PrintScrollableInteger(this Console instance, int x, int y, string prefaceText, ref int number, bool asString = false, int min = int.MinValue, int max = int.MaxValue, int baseStep = 1, int shiftStep = 5, int controlStep = 10, bool onlyPreface = false) {
            Point mousePos = new MouseScreenObjectState(instance, GameHost.Instance.Mouse).CellPosition;
            string numString = number.ToString();

            if (asString)
                numString = number.AsString();


            Rectangle clickableArea = new Rectangle(new Point(x, y), new Point(x + numString.Length + prefaceText.Length - 1, y));

            if (onlyPreface)
                clickableArea = new Rectangle(new Point(x, y), new Point(x + prefaceText.Length - 1, y));

            int mod = baseStep;
            if (EitherShift())
                mod *= shiftStep;
            if (EitherControl())
                mod *= controlStep;


            if (clickableArea.Contains(mousePos)) {
                if (GameHost.Instance.Mouse.ScrollWheelValueChange > 0 || GameHost.Instance.Mouse.RightClicked) {
                    number = Math.Clamp(number - mod, min, max);
                }
                else if (GameHost.Instance.Mouse.ScrollWheelValueChange < 0 || GameHost.Instance.Mouse.LeftClicked) {
                    number = Math.Clamp(number + mod, min, max);
                }
            }


            instance.Print(x, y, prefaceText);

            if (!onlyPreface)
                instance.Print(x + prefaceText.Length, y, numString);
        }

        public static void PrintStringField(this Console instance, int x, int y, string prefaceText, ref string field, ref string selected, string plainName, bool onlyPreface = false) {
            Point mousePos = new MouseScreenObjectState(instance, GameHost.Instance.Mouse).CellPosition;
            if (field == null)
                field = "";

            if (field != null) {
                Rectangle clickableArea = new Rectangle(new Point(x, y), new Point(x + field.Length + prefaceText.Length - 1, y));

                if (onlyPreface)
                    clickableArea = new Rectangle(new Point(x, y), new Point(x + prefaceText.Length - 1, y));

                int end = x + field.Length;
                if (field.Length == 0) {
                    clickableArea = new Rectangle(new Point(x, y), new Point(x + prefaceText.Length + "(blank)".Length - 1, y));
                    end = x + "(blank)".Length;
                    instance.Print(x, y, prefaceText, selected == plainName ? Color.Green : clickableArea.Contains(mousePos) ? Color.Yellow : Color.White);

                    if (!onlyPreface)
                        instance.Print(x + prefaceText.Length, y, "(blank)", clickableArea.Contains(mousePos) ? Color.Yellow : Color.White);
                }
                else {
                    instance.Print(x, y, prefaceText, selected == plainName ? Color.Green : clickableArea.Contains(mousePos) ? Color.Yellow : Color.White);

                    if (!onlyPreface)
                        instance.Print(x + prefaceText.Length, y, field, clickableArea.Contains(mousePos) ? Color.Yellow : Color.White);
                }

                if (GameHost.Instance.Mouse.LeftClicked) {
                    if (clickableArea.Contains(mousePos))
                        selected = plainName;
                }

                if (selected == plainName) {
                    if (CursorTicked + 200 < Time()) {
                        CursorTicked = Time();
                        CursorVisible = !CursorVisible;
                    }

                    if (!onlyPreface)
                        instance.Print(end + prefaceText.Length, y, Helper.CursorVisible ? "_" : " ");

                    foreach (var key in GameHost.Instance.Keyboard.KeysPressed) {
                        if ((key.Character >= 'A' && key.Character <= 'z') || (key.Character >= '0' && key.Character <= '9'
                            || key.Character == ';' || key.Character == ':' || key.Character == '|' || key.Character == '-' || key.Character == '+'
                            || key.Character == '.' || key.Character == ',' || key.Character == '(' || key.Character == ')' || key.Character == '\''
                            || key.Character == '!' || key.Character == '?')) {
                            field += key.Character;
                        }

                        if (key.Character == '$') {
                            field += (char)15;
                        }
                    }

                    if (GameHost.Instance.Keyboard.IsKeyPressed(Keys.Space)) {
                        field += " ";
                    }

                    if (GameHost.Instance.Keyboard.IsKeyPressed(Keys.Back)) {
                        if (field.Length > 0)
                            field = field[0..^1];
                    }

                    if (GameHost.Instance.Keyboard.IsKeyPressed(Keys.Enter)) {
                        selected = "none";
                    }
                }
            }
        }

        public static void PrintAdjustableInt(this Console instance, int x, int y, int width, ref int target, int min, int max) {
            Point mousePos = new MouseScreenObjectState(instance, GameHost.Instance.Mouse).CellPosition;
            Point minusPos = new Point(x, y);
            Point plusPos = new Point(x + 3 + width, y);

            if (width > 0) {
                instance.Print(x, y, "-", mousePos == minusPos ? Color.Yellow : Color.White);
                instance.Print(x + 2, y, target.ToString().PadLeft(width));
                instance.Print(x + 3 + width, y, "+", mousePos == plusPos ? Color.Yellow : Color.White);
            }
            else {
                plusPos = new Point(x + 3 + target.ToString().Length, y);
                instance.Print(x, y, "-", mousePos == minusPos ? Color.Yellow : Color.White);
                instance.Print(x + 2, y, target.ToString());
                instance.Print(x + 3 + target.ToString().Length, y, "+", mousePos == plusPos ? Color.Yellow : Color.White);
            }

            int mod = 1;

            if (EitherShift())
                mod *= 5;

            if (EitherControl())
                mod *= 10;

            if (GameHost.Instance.Mouse.LeftClicked && mousePos == minusPos) {
                target = Math.Clamp(target - mod, min, max);
            }

            if (GameHost.Instance.Mouse.LeftClicked && mousePos == plusPos) {
                target = Math.Clamp(target + mod, min, max);
            }
        }

        public static void PrintVertical(this Console instance, int x, int y, string str, bool down = true) {
            instance.PrintVertical(x, y, new ColoredString(str), down);
        }

        public static void PrintVertical(this Console instance, int x, int y, ColoredString str, bool down = true) {
            for (int i = 0; i < str.Length; i++) {
                int printY = y + (down ? i : -i);
                if (printY >= 0 && printY < instance.Height) {
                    instance.Print(x, printY, str[i].Glyph.AsString(), str[i].Foreground, str[i].Background);
                }
            }
        }

        public static void PrintClickableBool(this SadConsole.Console instance, int x, int y, string input, ref bool toggler) {
            instance.PrintClickableBool(x, y, new ColoredString(input), ref toggler);
        }

        public static void PrintClickableBool(this SadConsole.Console instance, int x, int y, ColoredString input, ref bool toggler) {
            Point mousePos = new MouseScreenObjectState(instance, GameHost.Instance.Mouse).CellPosition;

            ColoredString str = input + Helper.Checkmark(toggler);

            if (mousePos.X >= x && mousePos.X < x + str.Length && mousePos.Y == y) {
                instance.Print(x, y, str.GetDarker());
            }
            else {
                instance.Print(x, y, str);
            }

            if (GameHost.Instance.Mouse.LeftClicked) {
                if (mousePos.X >= x && mousePos.X < x + str.Length && mousePos.Y == y) {
                    toggler.Flip();
                }
            }
        }

        public static void PrintClickable(this SadConsole.Console instance, int x, int y, string str, Action<string> OnClick, string ID) {
            instance.PrintClickable(x, y, new ColoredString(str), OnClick, ID);
        }

        public static void PrintClickable(this SadConsole.Console instance, int x, int y, ColoredString str, Action<string> OnClick, string ID) {
            MouseScreenObjectState mouse = new MouseScreenObjectState(instance, GameHost.Instance.Mouse);
            Point mousePos = mouse.CellPosition;
            bool mouseOn = mouse.IsOnScreenObject;

            if (mousePos.X >= x && mousePos.X < x + str.Length && mousePos.Y == y) {
                instance.Print(x, y, str.GetDarker());
            }
            else {
                instance.Print(x, y, str);
            }

            if (GameHost.Instance.Mouse.LeftClicked && !ProcessedClick) {
                if (mousePos.X >= x && mousePos.X < x + str.Length && mousePos.Y == y && mouseOn) {
                    OnClick(ID);
                    ProcessedClick = true;
                }
            }
        }

        public static void PrintClickable<T1>(this Console instance, int x, int y, ColoredString str, Action<string, T1> OnClick, string ID, T1 arg) {
            MouseScreenObjectState mouse = new MouseScreenObjectState(instance, GameHost.Instance.Mouse);
            Point mousePos = mouse.CellPosition;
            bool mouseOn = mouse.IsOnScreenObject;

            int length = str.Length;
            var temp = str.ToArray();

            if (mousePos.X >= x && mousePos.X < x + str.Length && mousePos.Y == y) {
                instance.Print(x, y, str.GetDarker());
            }
            else {
                instance.Print(x, y, str);
            }

            if (GameHost.Instance.Mouse.LeftClicked && !ProcessedClick) {
                if (mousePos.X >= x && mousePos.X < x + length && mousePos.Y == y && mouseOn) {
                    OnClick(ID, arg);
                    ProcessedClick = true;
                } 
            }
        }

        public static void PrintClickable<T1, T2>(this SadConsole.Console instance, int x, int y, ColoredString str, Action<string, T1, T2> OnClick, string ID, T1 arg1, T2 arg2) {
            MouseScreenObjectState mouse = new MouseScreenObjectState(instance, GameHost.Instance.Mouse);
            Point mousePos = mouse.CellPosition;
            bool mouseOn = mouse.IsOnScreenObject;

            int length = str.Length;
            var temp = str.ToArray();

            if (mousePos.X >= x && mousePos.X < x + str.Length && mousePos.Y == y && mouseOn) {
                instance.Print(x, y, str.GetDarker());
            }
            else {
                instance.Print(x, y, str);
            }

            if (GameHost.Instance.Mouse.LeftClicked && !ProcessedClick) {
                if (mousePos.X >= x && mousePos.X < x + length && mousePos.Y == y) {
                    OnClick(ID, arg1, arg2);
                    ProcessedClick = true;
                } 
            }
        }

        public static void PrintClickable(this SadConsole.Console instance, int x, int y, string str, Action OnClick) {
            instance.PrintClickable(x, y, new ColoredString(str), OnClick);
        }

        public static void PrintClickable(this SadConsole.Console instance, int x, int y, ColoredString str, Action OnClick) {
            MouseScreenObjectState mouse = new MouseScreenObjectState(instance, GameHost.Instance.Mouse);
            Point mousePos = mouse.CellPosition;
            bool mouseOn = mouse.IsOnScreenObject;

            int length = str.Length - 1;

            instance.Print(x, y, mousePos.X >= x && mousePos.X <= x + length && mousePos.Y == y ? str.GetDarker() : str);

            if (GameHost.Instance.Mouse.LeftClicked) {
                if (mousePos.X >= x && mousePos.X <= x + length && mousePos.Y == y && mouseOn) {
                    OnClick();
                }
            }
        } 
         
        public static void SerializeToFile(object value, string path, JsonSerializerSettings settings = null) {
            using StreamWriter output = new StreamWriter(path);
            TypeDescriptor.AddAttributes(typeof(Point), new TypeConverterAttribute(typeof(PointConverter)));
            string jsonString = JsonConvert.SerializeObject(value, Formatting.Indented);
            output.WriteLine(jsonString);
            output.Close();
        }

        public static T DeserializeFromFile<T>(string path) {
            using StreamReader read = new StreamReader(path);
            TypeDescriptor.AddAttributes(typeof(Point), new TypeConverterAttribute(typeof(PointConverter)));
            T output = JsonConvert.DeserializeObject<T>(read.ReadToEnd());
            read.Close();
            return output;
        }

        public static void SerializeToFileCompressed(object value, string path) {
            File.WriteAllBytes(path, value.ToByteArray());
        }

        public static T DeserializeFromFileCompressed<T>(string path) { 
            return File.ReadAllBytes(path).FromByteArray<T>();
        } 

        public static byte[] ToByteArray(this object instance, JsonSerializerSettings settings = null) {
            MemoryStream stream = new();
            using (var writer = new StreamWriter(stream)) {
                var serializer = JsonSerializer.CreateDefault(settings);
                serializer.Serialize(writer, instance);
                writer.Close();
                stream.Close();
            }

            return stream.ToArray();
        }

        public static T FromByteArray<T>(this byte[] input, JsonSerializerSettings settings = null) {
            MemoryStream stream = new(input);
            T output;  

            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader)) {
                var serializer = JsonSerializer.CreateDefault(settings);
                
                output = serializer.Deserialize<T>(jsonReader);
                stream.Close();
                reader.Close();
            }

            return output;
        }
    } 

    public class PointConverter : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            //=> sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            => sourceType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            //var key = Convert.ToString(value)!.Trim('(').Trim(')');
            //var parts = Regex.Split(key, ",");

            var key = value.ToString().Trim('(').Trim(')');
            var parts = key.Split(",");

            return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
    
}
