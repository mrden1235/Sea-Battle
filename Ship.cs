using System;

namespace seaBattle
{
    public class Ship
    {
        public ShipPlace ShipPlace { get; set; } // место размещения коробля на поле
        public int Decks { get; }
        public Ship(int d)
        {
            if (d < 1 || d > 4) throw new ArgumentException();
            Decks = d;
        }
    }
}