using SplashKitSDK;


namespace Gta2D
{
    public class Zone
    {
        public const int ZONE_WIDTH = 10;
        public const int ZONE_HEIGHT = 5;

        private Tile[] _backgroundTiles;
        private List<Entity> _movingEntities;
        private List<Entity> _toBeAdded;
        private Json _json;

        public Zone(Point2D position)
        {
            _backgroundTiles = new Tile[ZONE_WIDTH * ZONE_HEIGHT];
            _movingEntities = [];
            _toBeAdded = [];

            for (int y = 0; y < ZONE_HEIGHT; y++)
            {
                for (int x = 0; x < ZONE_WIDTH; x++)
                {
                    _backgroundTiles[x + y * ZONE_WIDTH] = new Tile(x, y, new TileType
                    {
                        name = "water",
                        bitmap = "water",
                        collisionWidth = 50,
                        collisionHeight = 50
                    });
                    _backgroundTiles[x + y * ZONE_WIDTH].X = x * Tile.TILE_SIZE + position.X;
                    _backgroundTiles[x + y * ZONE_WIDTH].Y = y * Tile.TILE_SIZE + position.Y;
                }
            }

            _json = GetJson();
        }

        public Zone(Json json)
        {
            _backgroundTiles = new Tile[ZONE_WIDTH * ZONE_HEIGHT];
            _movingEntities = [];
            _toBeAdded = [];
            _json = json;

            List<double> position = [];
            json.ReadArray("position", ref position);

            List<string> tiles = [];
            json.ReadArray("tiles", ref tiles);

            for (int y = 0; y < ZONE_HEIGHT; y++)
            {
                for (int x = 0; x < ZONE_WIDTH; x++)
                {
                    string tileName = tiles[x + y * ZONE_WIDTH];
                    TileType? tile = Tile.GetTileTypes().Find(tile => tile.name == tileName);
                    if (tile is null) throw new NullReferenceException("Tile in level does not exist in tiles.json!");
                    _backgroundTiles[x + y * ZONE_WIDTH] = new Tile(x, y, tile);
                    _backgroundTiles[x + y * ZONE_WIDTH].X = x * Tile.TILE_SIZE + position[0];
                    _backgroundTiles[x + y * ZONE_WIDTH].Y = y * Tile.TILE_SIZE + position[1];
                }
            }

            Reset();
        }

        public List<Entity> MovingEntities { get { return _movingEntities; } }

        public void AddNewEntity(Entity e)
        {
            _toBeAdded.Add(e);
        }

        public void DrawTiles()
        {
            foreach (Tile tile in _backgroundTiles)
            {
                tile.Draw();
            }
        }

        public void DrawMovingEntities()
        {
            foreach (Entity e in _movingEntities)
            {
                e.Draw();
            }
        }

        public void Update(GameData gameData)
        {
            List<Entity> toBeRemoved = [];

            foreach (Entity e in _movingEntities)
            {
                e.Update(gameData);

                ILiving? living = e as ILiving;
                if (living != null)
                {
                    if (!living.IsAlive)
                    {
                        toBeRemoved.Add(e);

                        Police? police = e as Police;

                        Npc? npc = e as Npc; // e will always be an npc
                        if (npc == null) continue;

                        int wanted = gameData.wanted;

                        if (police == null)
                        {
                            gameData.money += npc.Money;

                            Point2D point;
                            do
                            {
                                point = Rng.RandomChoice(gameData.level.RespawnPoints);
                            }
                            while (SplashKit.PointOnScreen(SplashKit.ToScreen(point)));

                            Zone? zone = gameData.level.ZoneAt(point);
                            if (zone != null)
                            {
                                zone.AddNewEntity(NpcFactory.CreateNpc(point.X, point.Y));
                            }

                            if (wanted < 2)
                                wanted = 2;
                        }
                        else
                        {
                            gameData.money += npc.Money;

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

                            if (wanted == 4)
                                wanted = 5;
                            if (wanted < 4)
                                wanted = 4;
                        }

                        for (int i = 0; i < wanted - gameData.wanted; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                Point2D spawn;
                                do
                                {
                                    spawn = Rng.RandomChoice(gameData.level.RespawnPoints);
                                }
                                while (SplashKit.PointOnScreen(SplashKit.ToScreen(spawn)));

                                Zone? spawnZone = gameData.level.ZoneAt(spawn);
                                if (spawnZone != null)
                                {
                                    spawnZone.AddNewEntity(NpcFactory.CreatePolice(spawn.X, spawn.Y));
                                }
                            }
                        }

                        gameData.wanted = wanted;
                    }
                }
            }

            foreach (Entity e in toBeRemoved)
            {
                _movingEntities.Remove(e);
            }

            foreach (Entity e in _toBeAdded)
            {
                _movingEntities.Add(e);
            }
            _toBeAdded.Clear();
        }

        public Tile? GetBackgroundTile(double x, double y)
        {
            int nearest_x = Tile.NearestAxisPosition(x);
            int nearest_y = Tile.NearestAxisPosition(y);
            int x_offset = (nearest_x - (int)ZoneXAtX(x)) / Tile.TILE_SIZE;
            int y_offset = (nearest_y - (int)ZoneYAtY(y)) / Tile.TILE_SIZE;
            int index = x_offset + y_offset * ZONE_WIDTH;

            // do nothing if tile does not belong to the zone
            if (index < 0 || index >= _backgroundTiles.Length)
                return null;

            return _backgroundTiles[x_offset + y_offset * ZONE_WIDTH];
        }

        public void SetBackgroundTile(double x, double y, Tile tile)
        {
            int nearest_x = Tile.NearestAxisPosition(x);
            int nearest_y = Tile.NearestAxisPosition(y);
            int x_offset = (nearest_x - (int)ZoneXAtX(x)) / Tile.TILE_SIZE;
            int y_offset = (nearest_y - (int)ZoneYAtY(y)) / Tile.TILE_SIZE;
            int index = x_offset + y_offset * ZONE_WIDTH;

            // do nothing if tile does not belong to the zone
            if (index < 0 || index >= _backgroundTiles.Length)
                return;

            tile.X = nearest_x;
            tile.Y = nearest_y;
            _backgroundTiles[index] = tile;
        }

        public Json GetJson()
        {
            Json json = new Json();

            List<double> position = [_backgroundTiles[0].X, _backgroundTiles[0].Y];
            json.AddArray("position", position);

            List<string> tiles = [];
            foreach (Tile tile in _backgroundTiles)
            {
                tiles.Add(tile.Name);
            }
            json.AddArray("tiles", tiles);

            List<Json> cars = [];
            List<Json> npcs = [];
            List<Json> police = [];
            List<Json> shops = [];
            foreach (Entity e in _movingEntities)
            {
                Car? car = e as Car;
                if (car != null)
                {
                    cars.Add(car.GetJson());
                    continue;
                }

                Police? cop = e as Police;
                if (cop != null)
                {
                    police.Add(cop.GetJson());
                    continue;
                }

                Npc? npc = e as Npc;
                if (npc != null)
                {
                    npcs.Add(npc.GetJson());
                    continue;
                }

                Shop? shop = e as Shop;
                if (shop != null)
                {
                    shops.Add(shop.GetJson());
                    continue;
                }
            }
            json.AddArray("cars", cars);
            json.AddArray("npcs", npcs);
            json.AddArray("police", police);
            json.AddArray("shops", shops);

            return json;
        }

        public void Reset()
        {
            _movingEntities.Clear();
            _toBeAdded.Clear();

            List<Json> cars = [];
            _json.ReadArray("cars", ref cars);
            foreach (Json carJson in cars)
            {
                _movingEntities.Add(CarFactory.CreateCar(carJson));
            }

            List<Json> npcs = [];
            _json.ReadArray("npcs", ref npcs);
            foreach (Json npcJson in npcs)
            {
                _movingEntities.Add(NpcFactory.CreateNpc(npcJson));
            }

            List<Json> police = [];
            _json.ReadArray("police", ref police);
            foreach (Json policeJson in police)
            {
                _movingEntities.Add(NpcFactory.CreatePolice(policeJson));
            }

            List<Json> shops = [];
            _json.ReadArray("shops", ref shops);
            foreach (Json shopJson in shops)
            {
                _movingEntities.Add(ShopFactory.CreateShop(shopJson));
            }
        }

        public static void DrawEmptyZone(Point2D position)
        {
            SplashKit.FillRectangle(Color.Gray, position.X, position.Y, ZONE_WIDTH * Tile.TILE_SIZE, ZONE_HEIGHT * Tile.TILE_SIZE);
        }

        public static Point2D ZonePosAtPos(Point2D position)
        {
            return SplashKit.PointAt(ZoneXAtX((int)position.X), ZoneYAtY((int)position.Y));
        }

        public static double ZoneXAtX(double x)
        {
            return Math.Floor(x / (ZONE_WIDTH * Tile.TILE_SIZE)) * ZONE_WIDTH * Tile.TILE_SIZE;
        }

        public static double ZoneYAtY(double y)
        {
            return Math.Floor(y / (ZONE_HEIGHT * Tile.TILE_SIZE)) * ZONE_HEIGHT * Tile.TILE_SIZE;
        }
    }
}