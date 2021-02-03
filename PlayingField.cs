using System;
using System.Collections.Generic;

namespace seaBattle
{
    public class PlayingField
    {
        /// <summary>Массив кораблей.</summary>
        private readonly Ship[] _ships;
        /// <summary>Игровое поле.</summary>
        private readonly char[,] _field;
        /// <summary>Размеры игрового поля.</summary>
        private static int _n;
        private static int _m;
        /// <summary>конструктор</summary>
        /// <param name="n">Количество строк на игровом поле.</param>
        /// <param name="m">Количество столбцов на игровом поле.</param>
        /// <param name="s">Массив палуб.</param>
        public PlayingField(int n, int m, IReadOnlyList<int> s)
        {
            _n = n;
            _m = m;
            _field = new char[n, m];
            _ships = new Ship[s.Count];
            for (var i = 0; i < s.Count; ++i) _ships[i] = new Ship(s[i]);
        }
        /// <summary>Вывод игрового поля.</summary>
        public void PrintField()
        {
            for (var i = 0; i < _n; i++)
            {
                for (var j = 0; j < _m; j++)
                    Console.Write($"{_field[i, j]} \t");
                Console.WriteLine();
            }
        }

        /// <summary>Случайное распределение кораблей.</summary>
        public void StartRandomField(Random random)
        {
            var field = new char[_n + 2, _m + 2]; // создание увеличенного поля для удобства распределения кораблей

            for (var i = 0; i < _n + 2; i++) // заполнение поля
            for (var j = 0; j < _m + 2; j++)
                field[i, j] = '.';

            foreach (var s in _ships) ShipPlacement(field, s, random); // распределение кораблей

            for (var i = 0; i < _n; ++i) // запись в игровое поле
            for (var j = 0; j < _m; ++j)
                _field[i, j] = field[i + 1, j + 1];
        }

        /// <summary>Случайное распределение одного корабля.</summary>
        /// <param name="field">Увеличенное поле.</param>
        /// <param name="s">Корабль, который размещается на поле.</param>
        /// <param name="random">Объект класса Random.</param>
        private static void ShipPlacement(char[,] field, Ship s, Random random)
        {
            var shipPlaces = new List<ShipPlace>(); // список возможных мест для размещения корабля

            for (var i = 0; i < _n; ++i)
            for (var j = 0; j < _m; ++j)
            {
                for (var y = i; y < i + 3 && j < 11 - s.Decks; ++y) // нахождение возможных мест по вертикали
                {
                    var flag = false;
                    for (var x = j; x < j + s.Decks + 2; ++x)
                    {
                        if (field[y, x] == '*')
                        {
                            flag = true;
                            break;
                        }

                        if (y != i + 2 || x != j + s.Decks + 1) continue;
                        shipPlaces.Add(new ShipPlace(new Pointer(j, i), new Pointer(j + s.Decks - 1, i)));
                    }

                    if (flag) break;
                }

                for (var x = i; x < i + 3 && j < 11 - s.Decks; ++x)// нахождение возможных мест по горизонтали
                {
                    var flag = false;
                    for (var y = j; y < j + s.Decks + 2; ++y)
                    {
                        if (field[y, x] == '*')
                        {
                            flag = true;
                            break;
                        }

                        if (x != i + 2 || y != j + s.Decks + 1) continue;
                        shipPlaces.Add(new ShipPlace(new Pointer(i, j), new Pointer(i, j + s.Decks - 1)));
                    }

                    if (flag) break;
                }
            }

            s.ShipPlace = shipPlaces[random.Next(0, shipPlaces.Count - 1)]; // генерация случайного места для коробля

            for (var i = s.ShipPlace.FirstPointer.Y; // размещение корабля на поле
                i < s.ShipPlace.LastPointer.Y + 1;
                ++i)
            for (var j = s.ShipPlace.FirstPointer.X;
                j < s.ShipPlace.LastPointer.X + 1;
                ++j)
                field[i + 1, j + 1] = '*';

            if (shipPlaces.Count == 0) throw new Exception("Для корабля не нашлось подходящего места");
            shipPlaces.Clear();
        }
        /// <summary>Случайный выстрел.</summary>
        public void RandomShot(Random random)
        {
            var pointers = new List<Pointer>();
            for (var i = 0; i < _n; ++i)
            for (var j = 0; j < _m; ++j)
                if (_field[i, j] != 'x')
                    pointers.Add(new Pointer(j, i));
            var pointer = pointers[random.Next(0, pointers.Count - 1)];
            _field[pointer.Y, pointer.X] = 'x';
            pointers.Clear();
        }
        /// <summary>Проверка на убитого коробля в точке.</summary>
        /// /// <param name="x">Номер столбца.</param>
        /// <param name="y">Номер строки.</param>
        private bool DeathShip(int x, int y)
        {
            if (x < 0 || y < 0) return false;
            foreach (var s in _ships)
            {
                if (x == s.ShipPlace.FirstPointer.X &&  // проверка по вертикали
                    x == s.ShipPlace.LastPointer.X &&
                    y >= s.ShipPlace.FirstPointer.Y &&
                    y <= s.ShipPlace.LastPointer.Y)
                {
                    for (var j = 0;
                        j < s.Decks && _field[s.ShipPlace.FirstPointer.Y + j, x] == 'x';
                        ++j)
                        if (j == s.Decks - 1)
                            return true;
                }

                if (y != s.ShipPlace.FirstPointer.Y ||  // проверка по горизонтали
                    y != s.ShipPlace.LastPointer.Y ||
                    x < s.ShipPlace.FirstPointer.X ||
                    x > s.ShipPlace.LastPointer.X) continue;
                for (var j = 0;
                    j < s.Decks && _field[y, s.ShipPlace.FirstPointer.X + j] == 'x';
                    ++j)
                    if (j == s.Decks - 1)
                        return true;
            }

            return false;
        }
        /// <summary>Две точки для запоминания предыдущих выстрелов.</summary>
        private Pointer _prevHit;
        private Pointer _prevHit2; 
        /// <summary>Направление многопалубного корабля.</summary>
        private char _direction = ' ';
        /// <summary>Случайная стрельба с «добиванием» кораблей.</summary>
        public void SmartRandomShot(Random random)
        {
            if (_prevHit == null && _prevHit2 == null) // случайный выстрел
            {
                var pointers = new List<Pointer>();
                for (var i = 0; i < _n; ++i)
                for (var j = 0; j < _m; ++j)
                    if (_field[i, j] != 'x'
                       && !DeathShip(j - 1, i) 
                       && !DeathShip(j + 1, i) 
                       && !DeathShip(j, i - 1) 
                       && !DeathShip(j, i + 1)
                       && !DeathShip(j - 1, i - 1)
                       && !DeathShip(j + 1, i + 1)
                       && !DeathShip(j + 1, i - 1)
                       && !DeathShip(j - 1, i + 1))
                        pointers.Add(new Pointer(j, i));
                var pointer = pointers[random.Next(0, pointers.Count - 1)];
                if (_field[pointer.Y, pointer.X] == '*')
                    _prevHit = new Pointer(pointer.X, pointer.Y);
                _field[pointer.Y, pointer.X] = 'x';
                pointers.Clear();
            }
            else
                switch (_direction)
                {
                    case ' ' when _prevHit != null:
                        if (_prevHit.Y > 0)
                            switch (_field[_prevHit.Y - 1, _prevHit.X])
                            {
                                // выстрел над перым попадением
                                case '*':
                                {
                                    _field[_prevHit.Y - 1, _prevHit.X] = 'x';
                                    if (DeathShip(_prevHit.X, _prevHit.Y))
                                    {
                                        _prevHit = null;
                                        return;
                                    }

                                    _prevHit2 = new Pointer(_prevHit);
                                    _direction = 'y';
                                    --_prevHit.Y;
                                    return;
                                }
                                case '.':
                                    _field[_prevHit.Y - 1, _prevHit.X] = 'x';
                                    return;
                            }

                        if (_prevHit.X < _m - 1)
                            switch (_field[_prevHit.Y, _prevHit.X + 1])
                            {
                                // выстрел справа от первого попадения
                                case '*':
                                {
                                    _field[_prevHit.Y, _prevHit.X + 1] = 'x';
                                    if (DeathShip(_prevHit.X, _prevHit.Y))
                                    {
                                        _prevHit = null;
                                        return;
                                    }

                                    _prevHit2 = new Pointer(_prevHit);
                                    _direction = 'x';
                                    ++_prevHit.X;
                                    return;
                                }
                                case '.':
                                    _field[_prevHit.Y, _prevHit.X + 1] = 'x';
                                    return;
                            }

                        if (_prevHit.Y < _n - 1)
                            switch (_field[_prevHit.Y + 1, _prevHit.X])
                            {
                                // выстрел под первым попадением
                                case '*':
                                {
                                    _field[_prevHit.Y + 1, _prevHit.X] = 'x';
                                    if (DeathShip(_prevHit.X, _prevHit.Y))
                                    {
                                        _prevHit = null;
                                        return;
                                    }

                                    _direction = 'y';
                                    _prevHit2 = new Pointer(_prevHit.X, _prevHit.Y + 1);
                                    _prevHit = null;
                                    return;
                                }
                                case '.':
                                    _field[_prevHit.Y + 1, _prevHit.X] = 'x';
                                    return;
                            }

                        if (_prevHit.X > 0) // выстрел слева от первого попадения
                        {
                            switch (_field[_prevHit.Y, _prevHit.X - 1])
                            {
                                case '*':
                                {
                                    _field[_prevHit.Y, _prevHit.X - 1] = 'x';
                                    if (DeathShip(_prevHit.X, _prevHit.Y))
                                    {
                                        _prevHit = null;
                                        return;
                                    }

                                    _direction = 'x';
                                    _prevHit2 = new Pointer(_prevHit.X - 1, _prevHit.Y);
                                    _prevHit = null;
                                    break;
                                }
                                case '.':
                                    _field[_prevHit.Y, _prevHit.X - 1] = 'x';
                                    break;
                                default:
                                    _prevHit = null;
                                    SmartRandomShot(random);
                                    break;
                            }
                        }
                        else
                        {
                            _prevHit = null;
                            SmartRandomShot(random);
                        }

                        break;
                    case 'x' when _prevHit != null: // выстрелы справа от второго попадания
                        if (_prevHit.X < _m - 1)
                        {
                            switch (_field[_prevHit.Y, _prevHit.X + 1])
                            {
                                case '*':
                                {
                                    _field[_prevHit.Y, _prevHit.X + 1] = 'x';
                                    if (DeathShip(_prevHit.X, _prevHit.Y))
                                    {
                                        _prevHit = null;
                                        _prevHit2 = null;
                                        _direction = ' ';
                                    }
                                    else ++_prevHit.X;

                                    break;
                                }
                                case '.':
                                    _field[_prevHit.Y, _prevHit.X + 1] = 'x';
                                    _prevHit = null;
                                    break;
                                default:
                                    _prevHit = null;
                                    SmartRandomShot(random);
                                    break;
                            }
                        }
                        else
                        {
                            _prevHit = null;
                            SmartRandomShot(random);
                        }

                        break;
                    case 'y' when _prevHit != null: // выстрелы над вторым попадением
                        if (_prevHit.Y > 0)
                        {
                            switch (_field[_prevHit.Y - 1, _prevHit.X])
                            {
                                case '*':
                                {
                                    _field[_prevHit.Y - 1, _prevHit.X] = 'x';
                                    if (DeathShip(_prevHit.X, _prevHit.Y))
                                    {
                                        _prevHit = null;
                                        _prevHit2 = null;
                                        _direction = ' ';
                                    }
                                    else --_prevHit.Y;

                                    break;
                                }
                                case '.':
                                    _field[_prevHit.Y - 1, _prevHit.X] = 'x';
                                    _prevHit = null;
                                    break;
                                default:
                                    _prevHit = null;
                                    SmartRandomShot(random);
                                    break;
                            }
                        }
                        else
                        {
                            _prevHit = null;
                            SmartRandomShot(random);
                        }

                        break;
                    case 'y': // добивание нижней части корабля
                    {
                        _field[_prevHit2.Y + 1, _prevHit2.X] = 'x';
                        if (DeathShip(_prevHit2.X, _prevHit2.Y))
                        {
                            _prevHit2 = null;
                            _direction = ' ';
                        }
                        else ++_prevHit2.Y;

                        break;
                    }
                    case 'x': // добивание левой части корабля
                    {
                        _field[_prevHit2.Y, _prevHit2.X - 1] = 'x';
                        if (DeathShip(_prevHit2.X, _prevHit2.Y))
                        {
                            _prevHit2 = null;
                            _direction = ' ';
                        }
                        else --_prevHit2.X;

                        break;
                    }
                }
        }
        /// <summary>Проверка кораблей на живучесть.</summary>
        public bool Live()
        {
            for (var i = 0; i < _n; ++i)
            for (var j = 0; j < _m; ++j)
                if (_field[i, j] == '*')
                    return true;
            return false;
        }
    }
}