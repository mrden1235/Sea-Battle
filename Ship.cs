using System;

namespace seaBattle
{
    public class Ship
    {
        /// <summary>количество палуб</summary>
        public int Decks { get; }

        /// <summary>место размещения коробля на поле</summary>
        private ShipPlace _shipPlace;

        public ShipPlace ShipPlace
        {
            get => _shipPlace;
            set
            {
                if (Decks - 1 != Math.Abs(value.FirstPointer.X - value.LastPointer.X) &&
                    Decks - 1 != Math.Abs(value.FirstPointer.Y - value.LastPointer.Y))
                    throw new ArgumentException();
                if (0 != Math.Abs(value.FirstPointer.X - value.LastPointer.X) &&
                    0 != Math.Abs(value.FirstPointer.Y - value.LastPointer.Y))
                    throw new ArgumentException();
                _shipPlace = value;
            }

        }

        /// <summary>конструктор</summary>
        public Ship(int d)
        {
            if (d < 1) throw new ArgumentException();
            Decks = d;
        }
    }
}