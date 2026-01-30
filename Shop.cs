using SplashKitSDK;

namespace Gta2D
{
    public class Shop : Entity
    {
        private int _id;

        public Shop(double x, double y, int id) : base(x, y, new Sprite("empty"), Tile.TILE_SIZE, Tile.TILE_SIZE)
        {
            _id = id;
        }

        public void Enter()
        {
            Game.PushState(new ShopState(X, Y, _id));
        }

        public Json GetJson()
        {
            Json json = new Json();

            json.AddNumber("id", _id);
            json.AddNumber("x", X);
            json.AddNumber("y", Y);

            return json;
        }

        public override void Draw()
        {
            // if in the editior
            if (!Game.IsRunning())
            {
                SplashKit.FillQuad(Color.RGBAColor(0, 1, 0, 0.5), Hitbox);
            }
        }
    }
}