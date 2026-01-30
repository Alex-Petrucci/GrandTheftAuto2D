using SplashKitSDK;

namespace Gta2D
{
    public class CarFactory
    {
        public static Car CreateCar(double x, double y, string bitmap)
        {
            return new Car(
                x: x,
                y: y,
                acceleration: 0.8,
                maxSpeed: 7,
                turnSpeed: 3,
                bitmapName: bitmap
            );
        }

        public static Car CreateCar(Json json)
        {
            return new Car(
                x: json.ReadDouble("x"),
                y: json.ReadDouble("y"),
                acceleration: json.ReadDouble("acceleration"),
                maxSpeed: json.ReadDouble("max speed"),
                turnSpeed: json.ReadDouble("turn speed"),
                bitmapName: json.ReadString("bitmap")
            );
        }
    }
}