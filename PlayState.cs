using SplashKitSDK;

namespace Gta2D
{
    public class PlayState : IGameState
    {
        public void Enter(GameData gameData)
        {
            Game.PushState(new HudState());
        }

        public void Update(GameData gameData)
        {
            if (!gameData.paused)
            {
                if (SplashKit.KeyTyped(KeyCode.EscapeKey))
                {
                    Game.PushState(new PauseState());
                }

                gameData.player.Update(gameData);
                gameData.level.Update(gameData);
                gameData.level.MoveEntitiesIntoCorrectZones();
                SplashKit.CenterCameraOn(gameData.player.Sprite, 25, 25);
            }
        }

        public void Draw(GameData gameData)
        {
            gameData.level.DrawTiles();
            gameData.level.DrawMovingEntities();
            gameData.player.Draw();
        }
    }
}