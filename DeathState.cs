using SplashKitSDK;

namespace Gta2D
{
    public class DeathState : IGameState
    {
        public void Enter(GameData gameData)
        {
            gameData.paused = true;
        }

        public void Leave(GameData gameData)
        {
            gameData.Reset();
        }

        public void Update(GameData gameData)
        {
            if (SplashKit.KeyTyped(KeyCode.SpaceKey))
            {
                Game.PopState();
            }
        }
        
        public void Draw(GameData gameData)
        {
            Point2D camera = SplashKit.CameraPosition();
            SplashKit.SetCameraPosition(SplashKit.PointAt(0, 0));

            SplashKit.FillRectangle(Color.RGBAColor(0, 0, 0, 0.5), 0, 0, 640, 480);
            SplashKit.DrawText("Wasted", Color.White, SplashKit.FontNamed("uiFont"), 36, 10, 10);
            SplashKit.DrawText("Press [Space] to continue.", Color.White, SplashKit.FontNamed("uiFont"), 12, 10, 100);
            SplashKit.SetCameraPosition(camera);
        }
    }
}