using SplashKitSDK;

namespace Gta2D
{
    public class Level
    {
        Dictionary<Point2D, Zone> _zones;
        List<Point2D> _respawnPoints;

        public Level()
        {
            _zones = [];
            _respawnPoints = [];

            Json levelJson = SplashKit.JsonFromFile("level.json");

            List<Json> jsonZones = [];
            levelJson.ReadArray("zones", ref jsonZones);

            foreach (Json zone in jsonZones)
            {
                List<double> position = [];
                zone.ReadArray("position", ref position);

                _zones[SplashKit.PointAt(position[0], position[1])] = new Zone(zone);
            }

            List<Json> jsonRespawns = [];
            levelJson.ReadArray("respawn points", ref jsonRespawns);

            foreach (Json point in jsonRespawns)
            {
                double x = point.ReadNumber("x");
                double y = point.ReadNumber("y");

                _respawnPoints.Add(SplashKit.PointAt(x, y));
            }
        }

        public List<Point2D> RespawnPoints
        {
            get { return _respawnPoints; }
        }

        public void Update(GameData gameData)
        {
            foreach (Zone zone in GetZonesOnScreen(100))
            {
                zone.Update(gameData);
            }
        }

        public void DrawTiles()
        {
            foreach (Zone zone in GetZonesOnScreen(0))
            {
                zone.DrawTiles();
            }
        }

        public void DrawMovingEntities()
        {
            foreach (Zone zone in GetZonesOnScreen(100))
            {
                zone.DrawMovingEntities();
            }
        }

        public void DrawRespawnPoints()
        {
            foreach (Point2D point in _respawnPoints)
            {
                SplashKit.FillRectangle(Color.RGBAColor(1, 1, 0, 0.5), point.X, point.Y, Tile.TILE_SIZE, Tile.TILE_SIZE);
            }
        }

        public void AddZone(Zone zone, Point2D position)
        {
            _zones[position] = zone;
        }

        public Zone? ZoneAt(Point2D position)
        {
            try
            {
                return _zones[Zone.ZonePosAtPos(position)];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public Zone? ZoneAt(double x, double y)
        {
            return ZoneAt(SplashKit.PointAt(x, y));
        }

        public void Save()
        {
            Json json = new Json();

            List<Json> zonesJson = [];
            foreach (Zone zone in _zones.Values)
            {
                zonesJson.Add(zone.GetJson());
            }

            json.AddArray("zones", zonesJson);

            List<Json> respawnsJson = [];
            foreach (Point2D point in _respawnPoints)
            {
                Json pointJson = new Json();

                pointJson.AddNumber("x", point.X);
                pointJson.AddNumber("y", point.Y);

                respawnsJson.Add(pointJson);
            }

            json.AddArray("respawn points", respawnsJson);

            SplashKit.JsonToFile(json, "level.json");
        }

        public bool CollidesWith(Entity entity)
        {
            foreach (Point2D point in entity.Hitbox.Points)
            {
                if (CollidesWithTile(entity, point.X, point.Y))
                    return true;
            }
            return false;
        }

        public List<Zone> GetZonesOnScreen(int tolerance)
        {
            List<Zone> zones = [];

            for (double i = Zone.ZoneXAtX(SplashKit.CameraX() - tolerance); i <= Zone.ZoneXAtX(SplashKit.CameraX() + 640 + tolerance); i += Zone.ZONE_WIDTH * Tile.TILE_SIZE)
            {
                for (double j = Zone.ZoneYAtY(SplashKit.CameraY() - tolerance); j <= Zone.ZoneYAtY(SplashKit.CameraY() + 480 + tolerance); j += Zone.ZONE_HEIGHT * Tile.TILE_SIZE)
                {
                    Zone? zone = ZoneAt(i, j);
                    if (zone is not null) zones.Add(zone);
                }
            }

            return zones;
        }

        public void MoveEntitiesIntoCorrectZones()
        {
            foreach ((Point2D position, Zone zone) in _zones) 
            {
                bool repeat = true;
                while (repeat)
                {
                    repeat = false;
                    foreach (Entity e in zone.MovingEntities)
                    {
                        Zone? zoneWithEntity = ZoneAt(SplashKit.PointAt(e.X, e.Y));
                        Point2D zoneWithEntityPos = Zone.ZonePosAtPos(SplashKit.PointAt(e.X, e.Y));

                        Bullet? bullet = e as Bullet;

                        if (bullet != null)
                        {
                            if (bullet.ShouldRemove == true)
                            {
                                zone.MovingEntities.Remove(e);
                                repeat = true;
                                break;
                            }
                        }

                        if (zoneWithEntity == null)
                        {
                            zone.MovingEntities.Remove(e);
                            repeat = true;
                            break;
                        }

                        if (!position.Equals(zoneWithEntityPos))
                        {
                            zone.MovingEntities.Remove(e);
                            zoneWithEntity.MovingEntities.Add(e);
                            repeat = true;
                            break;
                        }
                    }
                }
            }
        }

        public void Reset()
        {
            foreach (Zone zone in _zones.Values)
            {
                zone.Reset();
            }
            MoveEntitiesIntoCorrectZones();
        }

        private bool CollidesWithTile(Entity entity, double x, double y)
        {
            Zone? zone = ZoneAt(x, y);
            if (zone is null) return false;

            Tile? tile = zone.GetBackgroundTile(x, y);
            if (tile is null) return false;

            return entity.CollidesWith(tile);
        }
    }
}