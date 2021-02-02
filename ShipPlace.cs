using System;

namespace seaBattle
{
    public class ShipPlace
    {
        /// <summary>первый конец корабля</summary>
        public Pointer FirstPointer { get; }

        /// <summary>второй конец корабля</summary>
        public Pointer LastPointer { get; }

        public ShipPlace(Pointer fp, Pointer lp)
        {
            if (0 != Math.Abs(fp.X - lp.X) && 0 != Math.Abs(fp.Y - lp.Y)) throw new ArgumentException();
            FirstPointer = fp;
            LastPointer = lp;
        }
    }
}