using SplashKitSDK;

namespace Gta2D
{
    public class MeleeWeapon : Weapon
    {
        double _radius;
        int _damage;
        int _speed;
        int _cooldown;
        bool _friendly;

        public MeleeWeapon(Sprite sprite, int speed, double radius, int damage, bool friendly) : base(0, 0, sprite)
        {
            _speed = speed;
            _radius = radius;
            _damage = damage;
            _cooldown = 0;
            _friendly = friendly;

            sprite.AnchorPoint = SplashKit.PointAt(Sprite.Width / 2, Sprite.Height / 2); // makes sure that the weapon rotates with the player correctly
        }

        public override void Update(GameData gameData)
        {
            _cooldown--;
        }

        public override void Attack(GameData gameData)
        {
            if (_cooldown <= 0)
            {
                _cooldown = _speed;

                Sprite.StartAnimation("fire");

                if (_friendly)
                {
                    foreach (Zone zone in gameData.level.GetZonesOnScreen(100))
                    {
                        foreach (Entity e in zone.MovingEntities)
                        {
                            ILiving? living = e as ILiving;
                            if (living == null)
                                continue;

                            if (SplashKit.CircleQuadIntersect(SplashKit.CircleAt(X + Sprite.Width/2, Y + Sprite.Height/2, _radius), e.Hitbox))
                            {
                                living.Hurt(_damage, gameData);
                            }
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            Sprite.UpdateAnimation();
            base.Draw();
        }
    }
}