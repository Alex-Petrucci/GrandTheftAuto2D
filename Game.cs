using SplashKitSDK;

namespace Gta2D
{
    public class Game
    {
        private static Game? s_instance;

        private Stack<IGameState> _gameStates;
        private Window _window;
        private GameData _gameData;
        private int _statesToBePopped;
        private List<IGameState> _statesToBePushed;

        private Game()
        {
            SplashKit.LoadResourceBundle("game bundle", "GameBundle.txt");
            Tile.LoadTileTypes();
            _gameStates = [];
            _window = new Window("Grand Theft Swinburne", 640, 480);

            _window.Clear(Color.Black);
            SplashKit.DrawText("Loading...", Color.White, "uiFont", 36, 10, 10);
            _window.Refresh();

            _gameData = new GameData();
            _statesToBePopped = 0;
            _statesToBePushed = [];
        }

        public static Game Instance
        {
            get
            {
                if (s_instance == null) s_instance = new Game();
                return s_instance;
            }
        }

        public static void Run()
        {
            PushState(new PlayState());

            while (!Instance._window.CloseRequested)
            {
                SplashKit.ProcessEvents();

                Instance._gameData.clickHandled = false;
                foreach (IGameState state in Instance._gameStates)
                {
                    state.Update(Instance._gameData);
                }

                SplashKit.ClearScreen();

                foreach (IGameState state in Instance._gameStates.Reverse())
                {
                    state.Draw(Instance._gameData);
                }

                // ensures that states are not modified while not being iterated over
                foreach (IGameState state in Instance._statesToBePushed.Slice(0, Instance._statesToBePushed.Count()))
                {
                    Instance._gameStates.Push(state);
                    state.Enter(Instance._gameData);
                    Instance._statesToBePushed.RemoveAt(0);
                }

                // ensures states are popped safely rather than while being used.
                for (int i = 0; i < Instance._statesToBePopped; i++)
                {
                    Instance._gameStates.Pop().Leave(Instance._gameData);
                }
                Instance._statesToBePopped = 0;

                SplashKit.RefreshScreen(60);

                if (Instance._gameStates.Count == 0)
                {
                    SplashKit.CloseCurrentWindow();
                }
            }
        }

        public static bool IsRunning()
        {
            return s_instance is not null;
        }

        public static void PushState(IGameState state)
        {
            Instance._statesToBePushed.Add(state);
        }

        public static void PopState()
        {
            Instance._statesToBePopped++;
        }
    }
}