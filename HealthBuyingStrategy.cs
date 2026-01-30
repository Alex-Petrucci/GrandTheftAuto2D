namespace Gta2D
{
    public class HealthBuyingStrategy : IBuyingStrategy
    {
        int _amount;

        public HealthBuyingStrategy(int amount)
        {
            _amount = amount;
        }

        public void Buy(GameData gameData)
        {
            gameData.player.Health += _amount;
            if (gameData.player.Health > 100)
                gameData.player.Health = 100;
        }
    }
}