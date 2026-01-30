namespace Gta2D
{
    public class CarBuyingStrategy : IBuyingStrategy
    {
        private double _x;
        private double _y;
        private double _acceleration;
        private double _maxSpeed;
        private double _turnSpeed;
        private string _bitmap;

        public CarBuyingStrategy(double x, double y, double acceleration, double maxSpeed, double turnSpeed, string bitmap)
        {
            _x = x;
            _y = y;
            _acceleration = acceleration;
            _maxSpeed = maxSpeed;
            _turnSpeed = turnSpeed;
            _bitmap = bitmap;
        }

        public void Buy(GameData gameData)
        {
            Zone? zone = gameData.level.ZoneAt(_x, _y);
            if (zone == null) return;

            zone.AddNewEntity(new Car(
                _x,
                _y,
                _acceleration,
                _maxSpeed,
                _turnSpeed,
                _bitmap
            ));
        }
    }
}