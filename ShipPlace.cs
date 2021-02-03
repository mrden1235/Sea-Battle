using System;

namespace seaBattle
{
    public class ShipPlace
    {
        /// <summary>Первый конец корабля.</summary>
        public Pointer FirstPointer { get; }

        /// <summary>Второй конец корабля.</summary>
        public Pointer LastPointer { get; }

        /// <summary>Конструктор.</summary>
        public ShipPlace(Pointer fp, Pointer lp)
        {
            if (0 != Math.Abs(fp.X - lp.X) && 0 != Math.Abs(fp.Y - lp.Y)) throw new ArgumentException();
            FirstPointer = fp;
            LastPointer = lp;
        }
    }
}