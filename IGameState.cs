namespace Gta2D
{
    public interface IGameState
    {
        public void Enter(GameData gameData) { }
        public void Leave(GameData gameData) { }
        public void Update(GameData gameData) { }
        public void Draw(GameData gameData) { }

    }
}