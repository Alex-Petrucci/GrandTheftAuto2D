using SplashKitSDK;

namespace Gta2D
{
    public class Police : Npc
    {
        private Weapon _weapon;

        public Police(double x, double y, int money, int health) : base(x, y, "police", money, health)
        {
            _weapon = new Gun(new Sprite("pistol", "pistolAnimation"), 1000, 30, 10, 10, false);
            _weapon.Sprite.StartAnimation("normal");
        }

        public override void Update(GameData gameData)
        {
            if (gameData.wanted == 0)
            {
                base.Update(gameData);
                _weapon.X = X;
                _weapon.Y = Y;
                _weapon.Rotation = Rotation;
                return;
            }

            Vector2D vel = SplashKit.VectorFromTo(Sprite, gameData.player.Sprite);
            vel = SplashKit.UnitVector(vel);
            vel = SplashKit.VectorMultiply(vel, 2.5);

            float rotation = (float)SplashKit.VectorAngle(vel) + 90;
            Rotation = 0; // prevents clipping

            string animation;

            if (SplashKit.PointPointDistance(Sprite.Position, gameData.player.Sprite.Position) > 200)
            {
                X += vel.X;
                while (gameData.level.CollidesWith(this))
                {
                    X -= Math.Sign(vel.X);
                }
                Y += vel.Y;
                while (gameData.level.CollidesWith(this))
                {
                    Y -= Math.Sign(vel.Y);
                }
                animation = "walk";
            }
            else
            {
                animation = "idle";
            }

            Rotation = rotation;
            _weapon.X = X;
            _weapon.Y = Y;
            _weapon.Rotation = Rotation;
            _weapon.Update(gameData);
            if (SplashKit.PointPointDistance(Sprite.Position, gameData.player.Sprite.Position) < 400)
                _weapon.Attack(gameData);

            _hurtFrames--;
            if (_hurtFrames > 0)
                SetAnimation("hurt");
            else
                SetAnimation(animation);
        }

        public override void Hurt(int damage, GameData gameData)
        {
            base.Hurt(damage, gameData);
            while (gameData.wanted < 3)
            {
                for (int i = 0; i < 5; i++)
                {
                    Point2D point;
                    do
                    {
                        point = Rng.RandomChoice(gameData.level.RespawnPoints);
                    }
                    while (SplashKit.PointOnScreen(SplashKit.ToScreen(point)));

                    Zone? zone = gameData.level.ZoneAt(point);
                    if (zone != null)
                    {
                        zone.AddNewEntity(NpcFactory.CreatePolice(point.X, point.Y));
                    }
                }
                gameData.wanted += 1;
            }
        }  

        public override void Draw()
        {
            base.Draw();
            _weapon.Draw();
        }
    }
}