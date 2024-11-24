using SadConsole;
using SadRogue.Primitives; 
using Console = SadConsole.Console;
using ElectricDreams.UI; 

namespace ElectricDreams {
    class GameLoop {
        public const int GameWidth = 150;
        public const int GameHeight = 50; 

        #pragma warning disable CS8618
        public static SadFont SquareFont; 
        public static UIManager UIManager; 
        public static Random rand; 
        #pragma warning restore CS8618 


        static void Main(string[] args) {
            Game.Create(GameWidth, GameHeight, "./fonts/ThinExtended.font");
            Game.Instance.OnStart = Init;
            Game.Instance.FrameUpdate += GlobalUpdate;

            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        private static void GlobalUpdate(object? sender, GameHost e) {  
            if (!GameHost.Instance.Mouse.LeftButtonDown) {
                Helper.ProcessedClick = false;
            }
        }

        private static void Init() {
            SquareFont = (SadFont)GameHost.Instance.LoadFont("./fonts/CheepicusExtended.font");
            Game.Instance.MonoGameInstance.Window.Title = "1980scape";
            rand = new(); 

            UIManager = new(); 
            UIManager.Init();  
        }
    }
}
