using SplashKitSDK;

namespace Gta2D
{
    public class GunBuyingStrategy : IBuyingStrategy
    {
        private string _bitmap;
        private double _range;
        private int _fireRate;
        private double _firePower;
        private int _damage;

        public GunBuyingStrategy(string bitmap, double range, int fireRate, double firePower, int damage)
        {
            _bitmap = bitmap;
            _range = range;
            _fireRate = fireRate;
            _firePower = firePower;
            _damage = damage;
        }

        public void Buy(GameData gameData)
        {
            gameData.player.Weapon = new Gun(
                new Sprite(_bitmap, $"{_bitmap}Animation"),
                _range,
                _fireRate,
                _firePower,
                _damage,
                true
            );
            gameData.player.Weapon.Sprite.StartAnimation("normal");
        }
    }
}