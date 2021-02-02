using System;

namespace seaBattle
{
    public class Pointer
    {
        // коордтнаты
        private int _x;

        public int X
        {
            get => _x;
            set
            {
                if (value < 0 || 9 < value) throw new ArgumentException();
                _x = value;
            }
        }

        private int _y;

        public int Y
        {
            get => _y;
            set
            {
                if (value < 0 || 9 < value) throw new ArgumentException();
                _y = value;
            }
        }

        // конструкторы
        public Pointer(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public Pointer(Pointer p)
        {
            _x = p._x;
            _y = p._y;
        }
    }
}