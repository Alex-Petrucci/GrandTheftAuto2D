using SplashKitSDK;

namespace Gta2D
{
    enum EntityType
    {
        Car,
        Npc,
        Police,
        PoliceCar
    }

    public class Editor
    {
        private static Editor? s_instance;

        private Window _window;
        private bool _dragStarted;
        private double _dragX;
        private double _dragY;
        private Level _level;
        private int _selectedTile;
        private string _currentLayer;
        private EntityType _entityType;
        private string _shopId;

        private Editor()
        {
            _window = new Window("Grand Theft Swinburne - Editor", 640, 480);
            _level = new Level();
            _selectedTile = 0;
            _currentLayer = "Background Tiles";
            _entityType = EntityType.Car;
            _shopId = "0";
        }

        public static void Run()
        {
            SplashKit.LoadResourceBundle("game bundle", "GameBundle.txt");
            Tile.LoadTileTypes();

            while (!Instance._window.CloseRequested)
            {
                SplashKit.ProcessEvents();
                SplashKit.ClearScreen();

                Instance.Update();
                Instance.Draw();

                SplashKit.RefreshScreen(60);
            }
        }

        public static Editor Instance
        {
            get
            {
                if (s_instance == null) s_instance = new Editor();
                return s_instance;
            }
        }

        private void Update()
        {
            UpdateUserInterface();

            Point2D mouse = SplashKit.ToWorld(SplashKit.MousePosition());
            double x = mouse.X;
            double y = mouse.Y;

            if (SplashKit.MouseDown(MouseButton.RightButton))
            {

                if (!_dragStarted)
                {
                    _dragStarted = true;
                    _dragX = x;
                    _dragY = y;
                }

                SplashKit.MoveCameraBy(_dragX - x, _dragY - y);
            }

            if (SplashKit.MouseUp(MouseButton.RightButton))
            {
                _dragStarted = false;
            }

            if (SplashKit.KeyTyped(KeyCode.ZKey))
            {
                if (_level.ZoneAt(mouse) == null)
                {
                    _level.AddZone(new Zone(Zone.ZonePosAtPos(mouse)), Zone.ZonePosAtPos(mouse));
                }
            }

            if (SplashKit.KeyDown(KeyCode.SpaceKey))
            {
                Zone? zone = _level.ZoneAt(mouse);

                if (_currentLayer == "Background Tiles")
                {
                    TileType tile = Tile.GetTileTypes()[_selectedTile];
                    if (zone != null)
                    {
                        zone.SetBackgroundTile(x, y, new Tile(0, 0, tile));
                    }
                }
            }

            if (SplashKit.KeyTyped(KeyCode.SpaceKey))
            {
                if (_currentLayer == "Moveable Entities")
                {
                    Zone? zone = _level.ZoneAt(mouse);
                    if (zone != null)
                    {
                        switch (_entityType)
                        {
                            case EntityType.Car:
                                zone.MovingEntities.Add(CarFactory.CreateCar(x, y, $"slowCar{Rng.GetRandomInt(1, 9)}"));
                                break;
                            case EntityType.Npc:
                                zone.MovingEntities.Add(NpcFactory.CreateNpc(x, y));
                                break;
                            case EntityType.Police:
                                zone.MovingEntities.Add(NpcFactory.CreatePolice(x, y));
                                break;
                            case EntityType.PoliceCar:
                                zone.MovingEntities.Add(CarFactory.CreateCar(x, y, "policeCar"));
                                break;
                        }
                    }
                }

                if (_currentLayer == "Shops")
                {
                    int id = int.Parse(_shopId);

                    int shop_x = Tile.NearestAxisPosition(x);
                    int shop_y = Tile.NearestAxisPosition(y);

                    Zone? zone = _level.ZoneAt(shop_x, shop_y);
                    if (zone != null)
                    {
                        zone.MovingEntities.Add(new Shop(shop_x, shop_y, id));
                    }
                }
                
                if (_currentLayer == "Respawn Points")
                {
                    int point_x = Tile.NearestAxisPosition(x);
                    int point_y = Tile.NearestAxisPosition(y);

                    _level.RespawnPoints.Add(SplashKit.PointAt(point_x, point_y));
                }
            }

            if (SplashKit.KeyTyped(KeyCode.SKey))
            {
                _level.Save();
            }
        }

        private void UpdateUserInterface()
        {
            if (SplashKit.StartPanel("Layer", SplashKit.RectangleFrom(400, 0, 240, 170)))
            {
                SplashKit.LabelElement($"Current: {_currentLayer}");
                if (SplashKit.Button("Background Tiles")) _currentLayer = "Background Tiles";
                if (SplashKit.Button("Moveable Entities")) _currentLayer = "Moveable Entities";
                if (SplashKit.Button("Shops")) _currentLayer = "Shops";
                if (SplashKit.Button("Respawn Points")) _currentLayer = "Respawn Points";
                SplashKit.EndPanel("Layer");
            }

            // Generated buttons can't be inside of panel because of SplashKit limitations
            if (_currentLayer == "Background Tiles")
            {
                int buttonX = 0;
                int buttonY = 0;
                int index = 0;
                foreach (TileType tile in Tile.GetTileTypes())
                {
                    Bitmap tileBitmap = SplashKit.BitmapNamed(tile.bitmap);
                    Rectangle buttonRect = SplashKit.RectangleFrom(buttonX, buttonY, 60, 60);
                    SplashKit.BitmapButton(tileBitmap, buttonRect);
                    // Manual click detection required because splashkit uses the line number for button ids which means all buttons are treated as the same button
                    if (SplashKit.MouseClicked(MouseButton.LeftButton) && SplashKit.PointInRectangle(SplashKit.MousePosition(), buttonRect))
                    {
                        _selectedTile = index;
                    }

                    buttonX += Tile.TILE_SIZE + 10;
                    if (buttonX > 200)
                    {
                        buttonX = 0;
                        buttonY += Tile.TILE_SIZE + 10;
                    }
                    index++;
                }
            }

            if (_currentLayer == "Moveable Entities")
            {
                if (SplashKit.StartPanel("Moveable Entities", SplashKit.RectangleFrom(0, 0, 150, 300)))
                {
                    SplashKit.LabelElement($"Current: {_entityType}");

                    if (SplashKit.Button("Car")) _entityType = EntityType.Car;
                    if (SplashKit.Button("NPC")) _entityType = EntityType.Npc;
                    if (SplashKit.Button("Police")) _entityType = EntityType.Police;
                    if (SplashKit.Button("Police Car")) _entityType = EntityType.PoliceCar;

                    SplashKit.EndPanel("Moveable Entities");
                }
            }

            if (_currentLayer == "Shops")
            {
                if (SplashKit.StartPanel("Shops", SplashKit.RectangleFrom(0, 0, 150, 300)))
                {
                    _shopId = SplashKit.TextBox("id", _shopId);
                    
                    SplashKit.EndPanel("Shops");
                }
            }
        }

        private void Draw()
        {
            Point2D mouse = SplashKit.ToWorld(SplashKit.MousePosition());
            double x = mouse.X;
            double y = mouse.Y;

            Zone.DrawEmptyZone(Zone.ZonePosAtPos(mouse));

            _level.DrawTiles();
            _level.DrawMovingEntities();
            _level.DrawRespawnPoints();

            int cursor_x = Tile.NearestAxisPosition(x);
            int cursor_y = Tile.NearestAxisPosition(y);

            SplashKit.DrawRectangle(Color.Red, cursor_x, cursor_y, Tile.TILE_SIZE, Tile.TILE_SIZE);

            SplashKit.DrawInterface();
        }
    }
}