using SplashKitSDK;

namespace Gta2D
{
    public static class NpcFactory
    {
        public static Npc CreateNpc(double x, double y)
        {
            return new Npc(x, y, $"npc{Rng.GetRandomInt(1,7)}", 100, 100);
        }

        public static Npc CreateNpc(Json json)
        {
            double x = json.ReadNumber("x");
            double y = json.ReadNumber("y");
            int money = (int)json.ReadNumber("money");
            int health = (int)json.ReadNumber("health");

            return new Npc(x, y, $"npc{Rng.GetRandomInt(1,6)}", money, health);
        }

        public static Police CreatePolice(double x, double y)
        {
            return new Police(x, y, 100, 100);
        }

        public static Police CreatePolice(Json json)
        {
            double x = json.ReadNumber("x");
            double y = json.ReadNumber("y");
            int money = (int)json.ReadNumber("money");
            int health = (int)json.ReadNumber("health");

            return new Police(x, y, money, health);
        }
    }
}