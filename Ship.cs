namespace seaBattle
{
    public class Ship
    {
        private ShipPlace _shipPlace; // место размещения коробля на поле
        private int _decks; // количество палуб
        
        //гетеры и сетеры
         public Ship(int d)
         { 
             _decks = d; 
         }
         
         public ShipPlace GetShipPlace() {
            return _shipPlace;
        }

        public void SetShipPlace(ShipPlace sp) {
            _shipPlace = sp;
        }
        
        public int GetDecks() {
            return _decks;
        }
        
        public void SetDecks(int d) {
            _decks = d;
        }
    }
}