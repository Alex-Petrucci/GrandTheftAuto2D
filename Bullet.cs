using SplashKitSDK;

namespace Gta2D
{
    public class Bullet : Entity
    {
        private double _range;
        private int _damage;
        private double _distance;
        private bool _shouldRemove;
        private bool _friendly;

        public Bullet(double x, double y, Sprite sprite, Vector2D movement, double range, int damage, bool friendly) : base(x, y, sprite, 15, 15)
        {
            Sprite.Velocity = movement;
            _range = range;
            _damage = damage;
            _distance = 0;
            _shouldRemove = false;
            _friendly = friendly;
        }

        public bool ShouldRemove { get { return _shouldRemove; } }

        public override void Update(GameData gameData)
        {
            Sprite.Move();
            _distance += SplashKit.VectorMagnitude(Sprite.Velocity);
            if (_distance > _range)
            {
                _shouldRemove = true;
            }
            if (gameData.level.CollidesWith(this))
            {
                _shouldRemove = true;
            }

            Zone? zone = gameData.level.ZoneAt(X + 5, Y + 5); // zone at the center of the bullet
            
            if (zone != null)
            {
                if (!_friendly)
                {
                    if (CollidesWith(gameData.player))
                    {
                        gameData.player.Hurt(_damage, gameData);
                        _shouldRemove = true;
                        return;
                    }
                }
                else
                {
                    foreach (Entity e in zone.MovingEntities)
                    {
                        ILiving? living = e as ILiving;

                        if (living != null)
                        {
                            if (CollidesWith(e))
                            {
                                living.Hurt(_damage, gameData);
                                _shouldRemove = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}