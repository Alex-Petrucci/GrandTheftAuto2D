using SplashKitSDK;

namespace Gta2D
{
    public abstract class Entity
    {
        protected Sprite _sprite;
        private double _hitboxWidth;
        private double _hitboxHeight;

        public Entity(double x, double y, Sprite sprite, double hitboxWidth, double hitboxHeight)
        {
            _sprite = sprite;
            _sprite.X = x;
            _sprite.Y = y;
            _hitboxWidth = hitboxWidth;
            _hitboxHeight = hitboxHeight;

            _sprite.AnchorPoint = SplashKit.PointAt(_sprite.Width / 2, Sprite.Height / 2);
        }

        public double X
        {
            get { return _sprite.X; }
            set { _sprite.X = value; }
        }
        public double Y
        {
            get { return _sprite.Y; }
            set { _sprite.Y = value; }
        }

        public float Rotation
        {
            get { return _sprite.Rotation; }
            set { _sprite.Rotation = value; }
        }

        public Quad Hitbox
        {
            get
            {
                Rectangle rect = _sprite.CollisionRectangle;
                double centerX = rect.X + rect.Width / 2;
                double centerY = rect.Y + rect.Height / 2;

                Vector2D forward = SplashKit.VectorFromAngle(Rotation - 90, 1);
                Vector2D right = SplashKit.VectorFromAngle(Rotation, 1);

                Vector2D widthVec = SplashKit.VectorMultiply(right, _hitboxWidth / 2);
                Vector2D heightVec = SplashKit.VectorMultiply(forward, _hitboxHeight / 2);

                Point2D topLeft = SplashKit.PointAt(centerX - widthVec.X - heightVec.X, centerY - widthVec.Y - heightVec.Y);
                Point2D topRight = SplashKit.PointAt(centerX + widthVec.X - heightVec.X, centerY + widthVec.Y - heightVec.Y);
                Point2D bottomLeft = SplashKit.PointAt(centerX - widthVec.X + heightVec.X, centerY - widthVec.Y + heightVec.Y);
                Point2D bottomRight = SplashKit.PointAt(centerX + widthVec.X + heightVec.X, centerY + widthVec.Y + heightVec.Y);

                return SplashKit.QuadFrom(
                    topLeft.X, topLeft.Y,
                    topRight.X, topRight.Y,
                    bottomLeft.X, bottomLeft.Y,
                    bottomRight.X, bottomRight.Y
                );
            }
        }

        public Sprite Sprite { get { return _sprite; } }

        public virtual void Draw()
        {
            _sprite.Draw();
        }

        public virtual void Update(GameData gameData)
        {

        }

        public void SetAnimation(string name)
        {
            if (Sprite.AnimationName() != name)
            {
                Sprite.StartAnimation(name);
            }
        }

        public bool CollidesWith(Entity other)
        {
            if (_hitboxWidth == 0 || _hitboxHeight == 0 || other._hitboxWidth == 0 || other._hitboxHeight == 0) return false;
            return SplashKit.QuadsIntersect(Hitbox, other.Hitbox);
        }
    }
}