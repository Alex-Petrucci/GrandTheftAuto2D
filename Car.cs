using SplashKitSDK;

namespace Gta2D
{
    public class Car : Entity
    {
        private double _acceleration;
        private double _velocity;
        private double _turnSpeed;
        private double _maxSpeed;
        private string _bitmapName;

        public Car(double x, double y, double acceleration, double maxSpeed, double turnSpeed, string bitmapName) : base(x, y, new Sprite(bitmapName), 80, 40)
        {
            _acceleration = acceleration;
            _velocity = 0.0;
            _turnSpeed = turnSpeed;
            _maxSpeed = maxSpeed;
            _bitmapName = bitmapName;

            Sprite.CollisionBitmap = SplashKit.BitmapNamed("carCollision");
        }

        public void Drive(GameData gameData)
        {
            if (SplashKit.KeyDown(KeyCode.AKey))
            {
                Rotation -= (float)(_turnSpeed * _velocity / _maxSpeed);
                if (gameData.level.CollidesWith(this))
                {
                    Rotation += (float)(_turnSpeed * _velocity / _maxSpeed);
                }
            }

            if (SplashKit.KeyDown(KeyCode.DKey))
            {
                Rotation += (float)(_turnSpeed * _velocity / _maxSpeed);
                if (gameData.level.CollidesWith(this))
                {
                    Rotation -= (float)(_turnSpeed * _velocity / _maxSpeed);
                }
            }

            if (SplashKit.KeyDown(KeyCode.WKey))
            {
                _velocity += _acceleration;
            }

            if (SplashKit.KeyDown(KeyCode.SKey))
            {
                _velocity -= _acceleration;
            }
        }

        public override void Update(GameData gameData)
        {
            _velocity *= 0.9;
            _velocity = Math.Clamp(_velocity, -_maxSpeed, _maxSpeed);
            double moveX = GetDirectionVector().X * _velocity;
            double moveY = GetDirectionVector().Y * _velocity;

            X += moveX;
            while (gameData.level.CollidesWith(this))
            {
                _velocity = 0;
                X -= moveX;
            }

            Y += moveY;
            while (gameData.level.CollidesWith(this))
            {
                _velocity = 0;
                Y -= moveY;
            }
        }

        public Json GetJson()
        {
            Json json = new Json();
            json.AddString("bitmap", _bitmapName);
            json.AddNumber("x", X);
            json.AddNumber("y", Y);
            json.AddNumber("acceleration", _acceleration);
            json.AddNumber("max speed", _maxSpeed);
            json.AddNumber("turn speed", _turnSpeed);
            return json;
        }

        private Vector2D GetDirectionVector()
        {
            return SplashKit.VectorFromAngle(Rotation, 1.0);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}