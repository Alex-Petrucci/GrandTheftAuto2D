using SplashKitSDK;

namespace Gta2D
{
    public record TileType
    {
        public required string name;
        public required string bitmap;
        public required double collisionWidth;
        public required double collisionHeight;
    }

    public class Tile : Entity
    {
        private static List<TileType> s_tileTypes = [];
        public const int TILE_SIZE = 50;

        private string _name;

        public Tile(double x, double y, TileType type) : base(x, y, new Sprite(type.bitmap), type.collisionWidth, type.collisionHeight)
        {
            _name = type.name;
        }

        public static int NearestAxisPosition(double position)
        {
            return ((int)position / TILE_SIZE - (position < 0 ? 1 : 0)) * TILE_SIZE;
        }

        public static void LoadTileTypes()
        {
            Json tilesJson = SplashKit.JsonFromFile("tiles.json");
            List<Json> tiles = [];
            tilesJson.ReadArray("tiles", ref tiles);

            foreach (Json tile in tiles)
            {
                s_tileTypes.Add(new TileType
                {
                    name = tile.ReadString("name"),
                    bitmap = tile.ReadString("bitmap"),
                    collisionWidth = tile.ReadNumber("collision width"),
                    collisionHeight = tile.ReadNumber("collision height")
                });
            }
        }

        public static List<TileType> GetTileTypes()
        {
            return s_tileTypes;
        }

        public string Name { get { return _name; } }
    }
}