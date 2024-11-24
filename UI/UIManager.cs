using SadConsole;
using SadConsole.UI;
using SadRogue.Primitives;
using Key = SadConsole.Input.Keys;

namespace _1980scape.UI {
    public class UIManager : ScreenObject {
        public SadConsole.UI.Colors? CustomColors;
        public Dictionary<string, InstantUI> Interfaces = new();

        public UI_GameWindow lofiHollow;

        public UIManager() {
            IsVisible = true;
            IsFocused = true;
            UseMouse = true;
            Parent = GameHost.Instance.Screen;
        } 
        public override void Update(TimeSpan timeElapsed) {
            bool anyVisible = false; 

            if (GameHost.Instance.Keyboard.KeysDown.Count == 0)
                Helper.ClearKeys();

            if (lofiHollow != null) {
                lofiHollow.Update();
                lofiHollow.Input();
            }

            base.Update(timeElapsed);
        }

        public void Init() {
            SetupCustomColors(); 
            lofiHollow = new UI_GameWindow(150, 50);
        }


        private void SetupCustomColors() {
            CustomColors = SadConsole.UI.Colors.CreateAnsi();
            CustomColors.ControlHostBackground = new AdjustableColor(Color.Black, "Black");
            CustomColors.Lines = new AdjustableColor(Color.White, "White");
            CustomColors.Title = new AdjustableColor(Color.White, "White");

            CustomColors.RebuildAppearances();
            SadConsole.UI.Themes.Library.Default.Colors = CustomColors;
        }
    }
}
