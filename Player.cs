using SplashKitSDK;

namespace Gta2D
{

    public class Player : Entity, ILiving
    {
        Car? _currentlyDriving;
        Weapon _weapon;
        int _health;
        int _hurtFrames;

        public Player(double x, double y) : base(x, y, new Sprite("player", "playerAnimation"), 40, 40)
        {
            Sprite.StartAnimation("idle");
            Sprite.AnchorPoint = SplashKit.PointAt(25, 25);
            _weapon = new MeleeWeapon(new Sprite("knife", "knifeAnimation"), 10, 30, 10, true);
            _weapon.Sprite.StartAnimation("normal");
            _health = 100;
        }

        public bool IsAlive
        {
            get { return _health > 0; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public Weapon Weapon
        {
            get { return _weapon; }
            set { _weapon = value; }
        }

        public void Hurt(int damage, GameData gameData)
        {
            _health -= damage;
            _hurtFrames = 40;
            Sprite.StartAnimation("hurt");

            if (_health <= 0)
            {
                Game.PushState(new DeathState());
            }
        }

        public override void Update(GameData gameData)
        {
            if (_currentlyDriving is not null)
            {
                _currentlyDriving.Drive(gameData);
                X = _currentlyDriving.X;
                Y = _currentlyDriving.Y;

                if (SplashKit.KeyTyped(KeyCode.EKey))
                {
                    _currentlyDriving = null;
                }
            }
            else
            {
                bool idle = true;

                if (SplashKit.KeyDown(KeyCode.WKey))
                {
                    Y -= 5;
                    idle = false;
                }
                while (gameData.level.CollidesWith(this))
                {
                    Y += 1;
                }

                if (SplashKit.KeyDown(KeyCode.SKey))
                {
                    Y += 5;
                    idle = false;
                }
                while (gameData.level.CollidesWith(this))
                {
                    Y -= 1;
                }

                if (SplashKit.KeyDown(KeyCode.AKey))
                {
                    X -= 5;
                    idle = false;
                }
                while (gameData.level.CollidesWith(this))
                {
                    X += 1;
                }

                if (SplashKit.KeyDown(KeyCode.DKey))
                {
                    X += 5;
                    idle = false;
                }
                while (gameData.level.CollidesWith(this))
                {
                    X -= 1;
                }

                if (SplashKit.KeyTyped(KeyCode.EKey))
                {
                    foreach (Zone zone in gameData.level.GetZonesOnScreen(100))
                    {
                        foreach (Entity e in zone.MovingEntities)
                        {
                            Car? car = e as Car;
                            if (car != null)
                            {
                                if (_sprite.SpriteCollision(car.Sprite))
                                {
                                    _currentlyDriving = car;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (SplashKit.KeyTyped(KeyCode.QKey))
                {
                    foreach (Zone zone in gameData.level.GetZonesOnScreen(0))
                    {
                        foreach (Entity entity in zone.MovingEntities)
                        {
                            Shop? shop = entity as Shop;
                            if (shop != null && shop.CollidesWith(this))
                            {
                                shop.Enter();
                            }
                        }
                    }
                }

                _weapon.Update(gameData);
                if (SplashKit.MouseDown(MouseButton.LeftButton) && !gameData.clickHandled)
                {
                    gameData.clickHandled = true;
                    _weapon.Attack(gameData);
                }

                _hurtFrames--;
                if (_hurtFrames > 0)
                {
                    SetAnimation("hurt");
                }
                else
                {
                    if (idle)
                    {
                        SetAnimation("idle");
                    }
                    else
                    {
                        SetAnimation("walk");
                    }
                }
            }
        }

        public override void Draw()
        {
            Point2D centerOfScreen = SplashKit.PointAt(640 / 2, 480 / 2);
            Point2D mousePos = SplashKit.MousePosition();
            Vector2D connectingVector = SplashKit.VectorPointToPoint(centerOfScreen, mousePos);
            Sprite.Rotation = (float)SplashKit.VectorAngle(connectingVector) + 90;

            Sprite.UpdateAnimation();
            if (_currentlyDriving is null)
            {
                base.Draw();
                _weapon.X = X;
                _weapon.Y = Y;
                _weapon.Rotation = Rotation;
                _weapon.Draw();
            }

            Sprite.Rotation = 0; // reset rotation otherwise clipping through walls happens sometimes
        }
    }
}