using SplashKitSDK;

namespace Gta2D
{
    public abstract class Weapon : Entity
    {
        public Weapon(double x, double y, Sprite sprite) : base(x, y, sprite, 0, 0)
        {

        }

        public abstract void Attack(GameData gameData);
    }
}