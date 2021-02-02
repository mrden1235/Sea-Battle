namespace seaBattle
{
    public class ShipPlace
    {
        public Pointer FirstPointer { get; }  // первый конец корабля
        public Pointer LastPointer { get; } // второй конец корабля
        public ShipPlace(Pointer fp, Pointer lp)
        {
            FirstPointer = fp;
            LastPointer = lp;
        }
    }
}