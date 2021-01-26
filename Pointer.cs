namespace seaBattle
{
    public class Pointer
    {
        // коордтнаты
        private int _x;
        private int _y;
        
        // конструкторы
        public Pointer(int x, int y) 
        {
            _x = x;
            _y = y;
        }

        public Pointer(Pointer p)
         {
             _x = p.GetX();
             _y = p.GetY();
         }

         //гетеры и сетеры
        public int GetX()
        {
            return _x;
        }
        
        public int GetY()
        {
            return _y;
        }

        public void SetX(int x)
        {
            _x = x;
        }
        
        public void SetY(int y)
        {
           _y = y;
        }
    }
}