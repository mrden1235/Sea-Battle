namespace seaBattle
{
    public class ShipPlace
    {
        private Pointer _firstPointer;  // первый конец корабля
        private Pointer _lastPointer; // второй конец корабля
        
        //гетеры и сетеры
        public ShipPlace(Pointer fp, Pointer lp)
        {
            _firstPointer = fp;
            _lastPointer = lp;
        }
        
        public Pointer GetFirstPointer()
        {
            return _firstPointer;
        }

        public void SetFirstPointer(Pointer fp)
        {
            _firstPointer = fp;
        }
        
        public Pointer GetLastPointer()
        {
            return _lastPointer;
        }

        public void SetLastPointer(Pointer lp)
        {
            _lastPointer = lp;
        }
        
    }
}