using SplashKitSDK;

namespace Gta2D
{
    public class Gun : Weapon
    {
        double _range;
        int _fireRate;
        double _firePower;
        int _damage;
        int _shotTimer;
        bool _friendly;

        public Gun(Sprite sprite, double range, int fireRate, double firePower, int damage, bool friendly) : base(0, 0, sprite)
        {
            _range = range;
            _fireRate = fireRate;
            _firePower = firePower;
            _damage = damage;
            _shotTimer = 0;
            _friendly = friendly;
        }

        public override void Update(GameData gameData)
        {
            _shotTimer--;
        }

        public override void Attack(GameData gameData)
        {
            if (_shotTimer <= 0)
            {
                Point2D centerOfScreen = SplashKit.PointAt(640 / 2, 480 / 2);
                Point2D mousePos = SplashKit.MousePosition();

                Vector2D connectingVector;
                if (_friendly)
                    connectingVector = SplashKit.VectorPointToPoint(centerOfScreen, mousePos);
                else
                    connectingVector = SplashKit.VectorPointToPoint(Sprite.Position, gameData.player.Sprite.Position);

                Vector2D bulletMovement = SplashKit.UnitVector(connectingVector);
                bulletMovement = SplashKit.VectorMultiply(bulletMovement, _firePower);

                double bulletX = X + 20 + bulletMovement.X * Tile.TILE_SIZE / 2 / _firePower;
                double bulletY = Y + 20 + bulletMovement.Y * Tile.TILE_SIZE / 2 / _firePower;

                Zone? zone = gameData.level.ZoneAt(bulletX, bulletY);
                if (zone == null) return;

                zone.AddNewEntity(new Bullet(bulletX, bulletY, new Sprite("bullet"), bulletMovement, _range, _damage, _friendly));

                Sprite.StartAnimation("fire");

                _shotTimer = _fireRate;
            }
        }

        public override void Draw()
        {
            Sprite.UpdateAnimation();
            base.Draw();
        }
    }
}