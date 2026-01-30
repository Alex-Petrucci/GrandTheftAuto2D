namespace Gta2D
{
    public class Purchasable
    {
        private IBuyingStrategy _buyingStrategy;
        private int _cost;
        private string _bitmap;
        private string _description;

        public Purchasable(IBuyingStrategy buyingStrategy, int cost, string bitmap, string description)
        {
            _buyingStrategy = buyingStrategy;
            _cost = cost;
            _bitmap = bitmap;
            _description = description;
        }
        
        public int Cost { get { return _cost; } }
        public string Bitmap { get { return _bitmap; } }
        public string Description { get { return _description; }}

        public void TryBuy(GameData gameData)
        {
            if (gameData.money >= _cost)
            {
                gameData.money -= _cost;
                _buyingStrategy.Buy(gameData);
            }
        }
    }
}