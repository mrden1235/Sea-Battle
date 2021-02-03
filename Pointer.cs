using System;

namespace seaBattle
{
    public class Pointer
    {
        /// <summary>Коордтнаты.</summary>
        private int _x;
        /// <exception>Выбрасывает исключение при отрицательной координате _x.</exception>
        public int X
        {
            get => _x;
            set
            {
                if (value < 0) throw new ArgumentException();
                _x = value;
            }
        }
        /// <exception>Выбрасывает исключение при отрицательной координате _y.</exception>
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

        /// <summary>Конструкторы.</summary>
        public Pointer(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Pointer(Pointer p)
        {
            X = p._x;
            Y = p._y;
        }
    }
}