using SplashKitSDK;

namespace Gta2D
{
    public class PauseState : IGameState
    {
        public void Enter(GameData gameData)
        {
            gameData.paused = true;
        }

        public void Leave(GameData gameData)
        {
            gameData.paused = false;
        }

        public void Update(GameData gameData)
        {
            if (SplashKit.KeyTyped(KeyCode.EscapeKey))
            {
                Game.PopState();
            }
        }
        
        public void Draw(GameData gameData)
        {
            Point2D camera = SplashKit.CameraPosition();
            SplashKit.SetCameraPosition(SplashKit.PointAt(0, 0));

            SplashKit.FillRectangle(Color.RGBAColor(0, 0, 0, 0.5), 0, 0, 640, 480);
            SplashKit.DrawText("Game Paused", Color.White, SplashKit.FontNamed("uiFont"), 36, 10, 10);

            SplashKit.SetCameraPosition(camera);
        }
    }
}