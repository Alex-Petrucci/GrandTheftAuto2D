using SplashKitSDK;

namespace Gta2D
{
    
    public class HudState : IGameState
    {
        private Bitmap _fullStarBitmap;
        private Bitmap _emptyStarBitmap;

        public HudState()
        {
            _fullStarBitmap = SplashKit.BitmapNamed("fullStar");
            _emptyStarBitmap = SplashKit.BitmapNamed("emptyStar");
        }

        public void Draw(GameData gameData)
        {
            Point2D camera = SplashKit.CameraPosition();
            SplashKit.SetCameraPosition(SplashKit.PointAt(0, 0));

            for (int i = 0; i < 5; i++)
            {
                if (gameData.wanted > i)
                    _fullStarBitmap.Draw(500 + i * 27, 400);
                else
                    _emptyStarBitmap.Draw(500 + i * 27, 400);
            }

            SplashKit.FillRectangle(SplashKit.StringToColor("#4c4c4cff"), 500, 430, 130, 20);
            SplashKit.FillRectangle(SplashKit.StringToColor("#f26c4fff"), 500, 430, 1.3 * gameData.player.Health, 20);
            SplashKit.DrawRectangle(Color.Black, 500, 430, 130, 20);

            SplashKit.DrawText($"${gameData.money}", Color.White, "uiFont", 24, 500, 445);

            SplashKit.SetCameraPosition(camera);
        }

        public void Leave(GameData gameData)
        {
            Game.PopState(); // ensures that play state is removed with this state
        }
    }
}