using SplashKitSDK;

namespace Gta2D
{
    public class ShopState : IGameState
    {
        private static int? s_openShop = null;

        private double _x;
        private double _y;
        private List<Purchasable> _items;

        public ShopState(double x, double y, int id)
        {
            _x = x;
            _y = y;
            _items = [];

            // don't open this shop if it is already open
            if (s_openShop == id)
            {
                Game.PopState();
                return;
            }

            s_openShop = id;

            Json json = SplashKit.JsonFromFile("shops.json");

            List<Json> shopsJson = [];
            json.ReadArray("shops", ref shopsJson);

            Json? thisShopJson = null;

            foreach (Json shopJson in shopsJson)
            {
                if (shopJson.ReadInteger("id") == id)
                {
                    thisShopJson = shopJson;
                    break;
                }
            }

            if (thisShopJson != null)
            {
                List<Json> itemsJson = [];
                thisShopJson.ReadArray("items", ref itemsJson);

                foreach (Json item in itemsJson)
                {
                    if (item.ReadString("type") == "car")
                    {
                        int price = item.ReadInteger("price");
                        string bitmap = item.ReadString("bitmap");
                        double acceleration = item.ReadDouble("acceleration");
                        double maxSpeed = item.ReadDouble("max speed");
                        double turnSpeed = item.ReadDouble("turn speed");
                        double spawnX = item.ReadDouble("spawn x");
                        double spawnY = item.ReadDouble("spawn y");

                        

                        _items.Add(new Purchasable(
                            new CarBuyingStrategy(_x + spawnX, _y + spawnY, acceleration, maxSpeed, turnSpeed, bitmap),
                            price,
                            $"{bitmap}Thumbnail",
                            $"Speed: {maxSpeed}, Acceleration: {acceleration}, Turning: {turnSpeed}"
                        ));
                    }

                    if (item.ReadString("type") == "gun")
                    {
                        int price = item.ReadInteger("price");
                        string bitmap = item.ReadString("bitmap");
                        double range = item.ReadNumber("range");
                        int fireRate = item.ReadInteger("fire rate");
                        double firePower = item.ReadNumber("fire power");
                        int damage = item.ReadInteger("damage");

                        _items.Add(new Purchasable(
                            new GunBuyingStrategy(bitmap, range, fireRate, firePower, damage),
                            price,
                            $"{bitmap}Thumbnail",
                            $"Damage: {damage}, Range: {range}, Speed: {fireRate}, Power: {firePower}"
                        ));
                    }

                    if (item.ReadString("type") == "melee")
                    {
                        int price = item.ReadInteger("price");
                        string bitmap = item.ReadString("bitmap");
                        int damage = item.ReadInteger("damage");
                        int speed = item.ReadInteger("speed");
                        double radius = item.ReadNumber("radius");

                        _items.Add(new Purchasable(
                            new MeleeBuyingStrategy(bitmap, speed, radius, damage),
                            price,
                            $"{bitmap}Thumbnail",
                            $"Damage: {damage}, Range: {radius}, Speed: {speed}"
                        ));
                    }
                    
                    if (item.ReadString("type") == "health")
                    {
                        int price = item.ReadInteger("price");
                        int amount = item.ReadInteger("amount");

                        _items.Add(new Purchasable(
                            new HealthBuyingStrategy(amount),
                            price,
                            "healthpack",
                            $"Heals: {amount}%"
                        ));
                    }
                }
            }
        }
        
        public void Update(GameData gameData)
        {
            if (SplashKit.PointPointDistance(SplashKit.PointAt(gameData.player.X, gameData.player.Y), SplashKit.PointAt(_x, _y)) > 150)
            {
                Game.PopState();
            }

            const int width = 170;
            const int height = 70;

            double x_pos = _x - width / 2 + 10;
            double y_pos = _y - height + 10;

            Point2D mouse = SplashKit.ToWorld(SplashKit.MousePosition());

            int offset = 10;
            foreach (Purchasable item in _items)
            {
                Rectangle rect = SplashKit.RectangleFrom(x_pos + offset, y_pos + 10, Tile.TILE_SIZE, Tile.TILE_SIZE);

                if (SplashKit.PointInRectangle(mouse, rect) &&
                    SplashKit.MouseClicked(MouseButton.LeftButton) &&
                    !gameData.clickHandled)
                {
                    item.TryBuy(gameData);
                }

                offset += 50;
            }

            if (SplashKit.MouseDown(MouseButton.LeftButton) &&
                SplashKit.PointInRectangle(mouse.X, mouse.Y, x_pos, y_pos, width, height))
            {
                gameData.clickHandled = true;
            }
        }

        public void Draw(GameData gameData)
        {
            const int width = 170;
            const int height = 70;

            double x_pos = _x - width / 2 + 10;
            double y_pos = _y - height + 10;

            SplashKit.FillRectangle(Color.DarkBlue, x_pos, y_pos, width, height);
            SplashKit.DrawRectangle(Color.LightBlue, x_pos, y_pos, width, height);

            int offset = 10;
            foreach (Purchasable item in _items)
            {
                Rectangle rect = SplashKit.RectangleFrom(x_pos + offset, y_pos + 10, Tile.TILE_SIZE, Tile.TILE_SIZE);

                SplashKit.DrawRectangle(Color.LightBlue, rect);

                Bitmap thumbnail = SplashKit.BitmapNamed(item.Bitmap);
                thumbnail.Draw(rect.X, rect.Y);

                if (SplashKit.PointInRectangle(SplashKit.ToWorld(SplashKit.MousePosition()), rect))
                {
                    SplashKit.FillRectangle(Color.RGBAColor(1, 1, 1, 0.5), rect);
                    SplashKit.DrawText($"${item.Cost}", Color.White, "uiFont", 12, x_pos, y_pos + 70);
                    SplashKit.DrawText(item.Description, Color.White, "uiFont", 12, x_pos, y_pos + 90);
                }

                offset += 50;
            }       
        }
        
        public void Leave(GameData gameData)
        {
            s_openShop = null;
        }
    }
}