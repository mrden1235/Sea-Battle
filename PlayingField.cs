using System;
using System.Collections.Generic;

namespace seaBattle
{
    public class PlayingField
    {
        private Ship[] _ships = new Ship[10]; // массив кораблей
        private char[,] _field = new char[10, 10]; // игровое поле
        
        //гетеры и сетеры
        public Ship[] GetShips()
        {
            return _ships;
        }

        public void SetShips(Ship[] s)
        {
            _ships = s;
        }

        public char[,] GetField()
        {
            return _field;
        }

        public void SetField(char[,] f)
        {
            _field = f;
        }
        
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
                for (var y = i; y < i + 3 && j < 11 - s.GetDecks(); ++y) // нахождение возможных мест по вертикали
                {
                    var flag = false;
                    for (var x = j; x < j + s.GetDecks() + 2; ++x)
                    {
                        if (field[y, x] == '*')
                        {
                            flag = true;
                            break;
                        }

                        if (y != i + 2 || x != j + s.GetDecks() + 1) continue;
                        shipPlaces.Add(new ShipPlace(new Pointer(j, i), new Pointer(j + s.GetDecks() - 1, i)));
                    }

                    if (flag) break;
                }

                for (var x = i; x < i + 3 && j < 11 - s.GetDecks(); ++x)// нахождение возможных мест по горизонтали
                {
                    var flag = false;
                    for (var y = j; y < j + s.GetDecks() + 2; ++y)
                    {
                        if (field[y, x] == '*')
                        {
                            flag = true;
                            break;
                        }

                        if (x != i + 2 || y != j + s.GetDecks() + 1) continue;
                        shipPlaces.Add(new ShipPlace(new Pointer(i, j), new Pointer(i, j + s.GetDecks() - 1)));
                    }

                    if (flag) break;
                }
            }

            s.SetShipPlace(shipPlaces[random.Next(0, shipPlaces.Count - 1)]); // генерация случайного места для коробля

            for (var i = s.GetShipPlace().GetFirstPointer().GetY(); // размещение корабля на поле
                i < s.GetShipPlace().GetLastPointer().GetY() + 1;
                ++i)
            for (var j = s.GetShipPlace().GetFirstPointer().GetX();
                j < s.GetShipPlace().GetLastPointer().GetX() + 1;
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
            _field[pointer.GetY(), pointer.GetX()] = 'x';
            pointers.Clear();
        }
        private bool DeathShip(Pointer p) // проверка на убитого коробля в точке
        {
            foreach (var t in _ships)
            {
                if (p.GetX() == t.GetShipPlace().GetFirstPointer().GetX() &&  // проверка по вертикали
                    p.GetX() == t.GetShipPlace().GetLastPointer().GetX() &&
                    p.GetY() >= t.GetShipPlace().GetFirstPointer().GetY() &&
                    p.GetY() <= t.GetShipPlace().GetLastPointer().GetY())
                {
                    for (var j = 0;
                        j < t.GetDecks() && _field[t.GetShipPlace().GetFirstPointer().GetY() + j, p.GetX()] == 'x';
                        ++j)
                        if (j == t.GetDecks() - 1)
                            return true;
                }

                if (p.GetY() != t.GetShipPlace().GetFirstPointer().GetY() ||  // проверка по горизонтали
                    p.GetY() != t.GetShipPlace().GetLastPointer().GetY() ||
                    p.GetX() < t.GetShipPlace().GetFirstPointer().GetX() ||
                    p.GetX() > t.GetShipPlace().GetLastPointer().GetX()) continue;
                for (var j = 0;
                    j < t.GetDecks() && _field[p.GetY(), t.GetShipPlace().GetFirstPointer().GetX() + j] == 'x';
                    ++j)
                    if (j == t.GetDecks() - 1)
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
                if (_field[pointer.GetY(), pointer.GetX()] == '*')
                    _prevHit = new Pointer(pointer.GetX(), pointer.GetY());
                _field[pointer.GetY(), pointer.GetX()] = 'x';
                pointers.Clear();
            }
            else
                switch (_direction)
                {
                    case ' ' when _prevHit != null:
                        if (_prevHit.GetY() > 0) // выстрел над перым попадением
                            if (_field[_prevHit.GetY() - 1, _prevHit.GetX()] == '*')
                            {
                                _field[_prevHit.GetY() - 1, _prevHit.GetX()] = 'x';
                                if (DeathShip(_prevHit))
                                {
                                    _prevHit = null;
                                    return;
                                }

                                _prevHit2 = new Pointer(_prevHit);
                                _direction = 'y';
                                _prevHit.SetY(_prevHit.GetY() - 1);
                                return;
                            }
                            else if (_field[_prevHit.GetY() - 1, _prevHit.GetX()] == '.')
                            {
                                _field[_prevHit.GetY() - 1, _prevHit.GetX()] = 'x';
                                return;
                            }

                        if (_prevHit.GetX() < 9) // выстрел справа от первого попадения
                            if (_field[_prevHit.GetY(), _prevHit.GetX() + 1] == '*')
                            {
                                _field[_prevHit.GetY(), _prevHit.GetX() + 1] = 'x';
                                if (DeathShip(_prevHit))
                                {
                                    _prevHit = null;
                                    return;
                                }

                                _prevHit2 = new Pointer(_prevHit);
                                _direction = 'x';
                                _prevHit.SetX(_prevHit.GetX() + 1);
                                return;
                            }
                            else if (_field[_prevHit.GetY(), _prevHit.GetX() + 1] == '.')
                            {
                                _field[_prevHit.GetY(), _prevHit.GetX() + 1] = 'x';
                                return;
                            }

                        if (_prevHit.GetY() < 9) // выстрел под первым попадением
                            if (_field[_prevHit.GetY() + 1, _prevHit.GetX()] == '*')
                            {
                                _field[_prevHit.GetY() + 1, _prevHit.GetX()] = 'x';
                                if (DeathShip(_prevHit))
                                {
                                    _prevHit = null;
                                    return;
                                }

                                _direction = 'y';
                                _prevHit2 = new Pointer(_prevHit.GetX(), _prevHit.GetY() + 1);
                                _prevHit = null;
                                return;
                            }
                            else if (_field[_prevHit.GetY() + 1, _prevHit.GetX()] == '.')
                            {
                                _field[_prevHit.GetY() + 1, _prevHit.GetX()] = 'x';
                                return;
                            }

                        if (_prevHit.GetX() > 0) // выстрел слева от первого попадения
                        {
                            if (_field[_prevHit.GetY(), _prevHit.GetX() - 1] == '*')
                            {

                                _field[_prevHit.GetY(), _prevHit.GetX() - 1] = 'x';
                                if (DeathShip(_prevHit))
                                {
                                    _prevHit = null;
                                    return;
                                }

                                _direction = 'x';
                                _prevHit2 = new Pointer(_prevHit.GetX() - 1, _prevHit.GetY());
                                _prevHit = null;
                            }
                            else if (_field[_prevHit.GetY(), _prevHit.GetX() - 1] == '.')
                                _field[_prevHit.GetY(), _prevHit.GetX() - 1] = 'x';
                            else
                            {
                                _prevHit = null;
                                SmartRandomShot(random);
                            }
                        }
                        else
                        {
                            _prevHit = null;
                            SmartRandomShot(random);
                        }

                        break;
                    case 'x' when _prevHit != null: // выстрелы справа от второго попадания
                        if (_prevHit.GetX() < 9) 
                        {
                            if (_field[_prevHit.GetY(), _prevHit.GetX() + 1] == '*')
                            {
                                _field[_prevHit.GetY(), _prevHit.GetX() + 1] = 'x';
                                if (DeathShip(new Pointer(_prevHit.GetX(), _prevHit.GetY())))
                                {
                                    _prevHit = null;
                                    _prevHit2 = null;
                                    _direction = ' ';
                                }
                                else _prevHit.SetX(_prevHit.GetX() + 1);
                            }
                            else if (_field[_prevHit.GetY(), _prevHit.GetX() + 1] == '.')
                            {
                                _field[_prevHit.GetY(), _prevHit.GetX() + 1] = 'x';
                                _prevHit = null;
                            }
                            else
                            {
                                _prevHit = null;
                                SmartRandomShot(random);
                            }
                        }
                        else
                        {
                            _prevHit = null;
                            SmartRandomShot(random);
                        }

                        break;
                    case 'y' when _prevHit != null: // выстрелы над вторым попадением
                        if (_prevHit.GetY() > 0)
                        {
                            if (_field[_prevHit.GetY() - 1, _prevHit.GetX()] == '*')
                            {
                                _field[_prevHit.GetY() - 1, _prevHit.GetX()] = 'x';
                                if (DeathShip(new Pointer(_prevHit.GetX(), _prevHit.GetY())))
                                {
                                    _prevHit = null;
                                    _prevHit2 = null;
                                    _direction = ' ';
                                }
                                else _prevHit.SetY(_prevHit.GetY() - 1);
                            }
                            else if (_field[_prevHit.GetY() - 1, _prevHit.GetX()] == '.')
                            {
                                _field[_prevHit.GetY() - 1, _prevHit.GetX()] = 'x';
                                _prevHit = null;
                            }
                            else
                            {
                                _prevHit = null;
                                SmartRandomShot(random);
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
                        _field[_prevHit2.GetY() + 1, _prevHit2.GetX()] = 'x';
                        if (DeathShip(new Pointer(_prevHit2.GetX(), _prevHit2.GetY())))
                        {
                            _prevHit2 = null;
                            _direction = ' ';
                        }
                        else _prevHit2.SetY(_prevHit2.GetY() + 1);

                        break;
                    }
                    case 'x': // добивание левой части корабля
                    {
                        _field[_prevHit2.GetY(), _prevHit2.GetX() - 1] = 'x';
                        if (DeathShip(new Pointer(_prevHit2.GetX(), _prevHit2.GetY())))
                        {
                            _prevHit2 = null;
                            _direction = ' ';
                        }
                        else _prevHit2.SetX(_prevHit2.GetX() - 1);

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