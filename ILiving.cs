namespace Gta2D
{
    public interface ILiving
    {
        public void Hurt(int damage, GameData gameData);

        public bool IsAlive { get; }
    }
}