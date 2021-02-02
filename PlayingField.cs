using System;
using System.Collections.Generic;

namespace seaBattle
{
    public class PlayingField
    {
        private readonly Ship[] _ships = new Ship[10]; // массив кораблей
        private readonly char[,] _field = new char[10, 10]; // игровое поле

        public void PrintField() // вывод игрового поля
        {
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                    Console.Write($"{_field[i, j]} \t");
                Console.WriteLine();
            }
        }

        public void StartRandomField(Random random) // случайное распределение кораблей
        {
            var field = new char[12, 12]; // создание увеличенного поля для удобства распределения кораблей

            for (var i = 0; i < 12; i++) // заполнение поля
            for (var j = 0; j < 12; j++)
                field[i, j] = '.';

            _ships[0] = new Ship(4); // создание кораблей
            _ships[1] = new Ship(3);
            _ships[2] = new Ship(3);
            _ships[3] = new Ship(2);
            _ships[4] = new Ship(2);
            _ships[5] = new Ship(2);
            _ships[6] = new Ship(1);
            _ships[7] = new Ship(1);
            _ships[8] = new Ship(1);
            _ships[9] = new Ship(1);

            for (var i = 0; i < 10; ++i) // распределение кораблей
                ShipPlacement(field, _ships[i], random); 

            for (var i = 0; i < 10; ++i) // запись в игровое поле
            for (var j = 0; j < 10; ++j)
                _field[i, j] = field[i + 1, j + 1]; 
        }

        private static void ShipPlacement(char[,] field, Ship s, Random random) // случайное распределение одного корабля
        {
            var shipPlaces = new List<ShipPlace>(); // список возможных мест для размещения корабля

            for (var i = 0; i < 10; ++i)
            for (var j = 0; j < 10; ++j)
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

            shipPlaces.Clear();
        }
        

        public void RandomShot(Random random) // случайный выстрел
        {
            var pointers = new List<Pointer>();
            for (var i = 0; i < 10; ++i)
            for (var j = 0; j < 10; ++j)
                if (_field[i, j] != 'x')
                    pointers.Add(new Pointer(j, i));
            var pointer = pointers[random.Next(0, pointers.Count - 1)];
            _field[pointer.Y, pointer.X] = 'x';
            pointers.Clear();
        }
        private bool DeathShip(Pointer p) // проверка на убитого коробля в точке
        {
            foreach (var t in _ships)
            {
                if (p.X == t.ShipPlace.FirstPointer.X &&  // проверка по вертикали
                    p.X == t.ShipPlace.LastPointer.X &&
                    p.Y >= t.ShipPlace.FirstPointer.Y &&
                    p.Y <= t.ShipPlace.LastPointer.Y)
                {
                    for (var j = 0;
                        j < t.Decks && _field[t.ShipPlace.FirstPointer.Y + j, p.X] == 'x';
                        ++j)
                        if (j == t.Decks - 1)
                            return true;
                }

                if (p.Y != t.ShipPlace.FirstPointer.Y ||  // проверка по горизонтали
                    p.Y != t.ShipPlace.LastPointer.Y ||
                    p.X < t.ShipPlace.FirstPointer.X ||
                    p.X > t.ShipPlace.LastPointer.X) continue;
                for (var j = 0;
                    j < t.Decks && _field[p.Y, t.ShipPlace.FirstPointer.X + j] == 'x';
                    ++j)
                    if (j == t.Decks - 1)
                        return true;
            }

            return false;
        }
        
        private Pointer _prevHit;  // две точки для запоминания предыдущих выстрелов
        private Pointer _prevHit2; 
        private char _direction = ' '; // направление многопалубного корабля 
        public void SmartRandomShot(Random random) // случайная стрельба с «добиванием» кораблей
        {
            if (_prevHit == null && _prevHit2 == null) // случайный выстрел
            {
                var pointers = new List<Pointer>();
                for (var i = 0; i < 10; ++i)
                for (var j = 0; j < 10; ++j)
                    if (_field[i, j] != 'x'
                       && !DeathShip(new Pointer(j - 1, i))
                            && !DeathShip(new Pointer(j + 1, i))
                               && !DeathShip(new Pointer(j, i - 1))
                                    && !DeathShip(new Pointer(j, i + 1)))
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
                                    if (DeathShip(_prevHit))
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

                        if (_prevHit.X < 9)
                            switch (_field[_prevHit.Y, _prevHit.X + 1])
                            {
                                // выстрел справа от первого попадения
                                case '*':
                                {
                                    _field[_prevHit.Y, _prevHit.X + 1] = 'x';
                                    if (DeathShip(_prevHit))
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

                        if (_prevHit.Y < 9)
                            switch (_field[_prevHit.Y + 1, _prevHit.X])
                            {
                                // выстрел под первым попадением
                                case '*':
                                {
                                    _field[_prevHit.Y + 1, _prevHit.X] = 'x';
                                    if (DeathShip(_prevHit))
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
                                    if (DeathShip(_prevHit))
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
                        if (_prevHit.X < 9)
                        {
                            switch (_field[_prevHit.Y, _prevHit.X + 1])
                            {
                                case '*':
                                {
                                    _field[_prevHit.Y, _prevHit.X + 1] = 'x';
                                    if (DeathShip(new Pointer(_prevHit.X, _prevHit.Y)))
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
                                    if (DeathShip(new Pointer(_prevHit.X, _prevHit.Y)))
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
                        if (DeathShip(new Pointer(_prevHit2.X, _prevHit2.Y)))
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
                        if (DeathShip(new Pointer(_prevHit2.X, _prevHit2.Y)))
                        {
                            _prevHit2 = null;
                            _direction = ' ';
                        }
                        else --_prevHit2.X;

                        break;
                    }
                }
        }

        public bool Live() // проверка кораблей на живучесть 
        {
            for (var i = 0; i < 10; ++i)
            for (var j = 0; j < 10; ++j)
                if (_field[i, j] == '*')
                    return true;
            return false;
        }
    }
}