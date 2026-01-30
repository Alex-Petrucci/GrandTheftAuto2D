namespace Gta2D
{
    public record GameData
    {
        public Player player = new Player(100, 100);
        public Level level = new Level();
        public int wanted = 0;
        public bool paused = false;
        public int money = 0;
        public bool clickHandled = false;

        public void Reset()
        {
            player = new Player(100, 100);
            wanted = 0;
            paused = false;
            money = 0;
            level.Reset();
        }
    }
}