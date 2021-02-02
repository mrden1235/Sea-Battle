using System;

namespace seaBattle
{
    public class Pointer
    {
        /// <summary>коордтнаты</summary>
        private int _x;
        public int X
        {
            get => _x;
            set
            {
                if (value < 0) throw new ArgumentException();
                _x = value;
            }
        }

        private int _y;
        public int Y
        {
            get => _y;
            set
            {
                if (value < 0) throw new ArgumentException();
                _y = value;
            }
        }

        /// <summary>конструкторы</summary>
        public Pointer(int x, int y)
        {
            if (x < 0 || y < 0) throw new ArgumentException();
            _x = x;
            _y = y;
        }

        public Pointer(Pointer p)
        {
            if (p._x < 0 || p._y < 0) throw new ArgumentException();
            _x = p._x;
            _y = p._y;
        }
    }
}