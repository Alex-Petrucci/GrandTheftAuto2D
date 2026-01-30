using SplashKitSDK;

namespace Gta2D
{
    public class MeleeBuyingStrategy : IBuyingStrategy
    {
        private string _bitmap;
        private int _speed;
        private double _radius;
        private int _damage;

        public MeleeBuyingStrategy(string bitmap, int speed, double radius, int damage)
        {
            _bitmap = bitmap;
            _speed = speed;
            _radius = radius;
            _damage = damage;
        }

        public void Buy(GameData gameData)
        {
            gameData.player.Weapon = new MeleeWeapon(
                new Sprite(_bitmap, $"{_bitmap}Animation"),
                _speed,
                _radius,
                _damage,
                true
            );

            gameData.player.Weapon.Sprite.StartAnimation("normal");
        }
    }
}