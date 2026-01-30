namespace Gta2D
{
    public static class Rng
    {
        private static Random s_random = new Random();

        public static int GetRandomInt(int min, int max)
        {
            return s_random.Next(min, max);
        }

        public static T RandomChoice<T>(List<T> list)
        {
            return list[GetRandomInt(0, list.Count)];
        }
    }
}