using SplashKitSDK;

namespace Gta2D
{
    public static class ShopFactory
    {
        public static Shop CreateShop(Json json)
        {
            int id = json.ReadInteger("id");
            double x = json.ReadNumber("x");
            double y = json.ReadNumber("y");

            return new Shop(x, y, id);
        }
    }
}