using SplashKitSDK;

namespace Gta2D
{
    public class Npc : Entity, ILiving
    {

        private int _money;
        private int _health;
        private int _movementTimer;
        private Vector2D _movement;
        protected int _hurtFrames;

        public Npc(double x, double y, string bitmap, int money, int health) : base(x, y, new Sprite(bitmap, "playerAnimation"), 40, 30)
        {
            Sprite.StartAnimation("walk");
            Sprite.CollisionBitmap = SplashKit.BitmapNamed("playerCollision");
            Sprite.CollisionKind = CollisionTestKind.PixelCollisions;

            _money = money;
            _health = health;
            _movementTimer = 0;
        }

        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }

        public bool IsAlive
        {
            get { return _health > 0; }
        }

        public Json GetJson()
        {
            Json json = new Json();

            json.AddNumber("money", _money);
            json.AddNumber("health", _health);
            json.AddNumber("x", X);
            json.AddNumber("y", Y);

            return json;
        }

        public override void Update(GameData gameData)
        {
            _movementTimer--;
            if (_movementTimer <= 0)
            {
                _movementTimer = 180;
                MoveInNewDirection();
            }

            X += _movement.X;
            if (gameData.level.CollidesWith(this))
            {
                X -= _movement.X;
                MoveInNewDirection();
            }

            Y += _movement.Y;
            if (gameData.level.CollidesWith(this))
            {
                Y -= _movement.Y;
                MoveInNewDirection();
            }

            _hurtFrames--;
            if (_hurtFrames <= 0)
                SetAnimation("walk");

            Sprite.Rotation = (float)SplashKit.VectorAngle(_movement) + 90;
        }

        public override void Draw()
        {
            Sprite.UpdateAnimation();
            base.Draw();
        }

        public virtual void Hurt(int damage, GameData gameData)
        {
            _health -= damage;
            Sprite.StartAnimation("hurt");
            _hurtFrames = 40;

            if (gameData.wanted < 1)
                gameData.wanted = 1;
        }

        private void MoveInNewDirection()
        {
            _movement = SplashKit.VectorFromAngle(Rng.GetRandomInt(0, 360), 2);
        }
    }
}