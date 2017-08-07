using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clock
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetAngle());
            Console.ReadLine();
        }

        static string GetAngle()
        {
            short _hour = 0, _minute = 0;
            Console.Write("Time: ");
            string _time = Console.ReadLine();
            string tmp = String.Empty;

            foreach (char ch in _time)
            {
                if (ch == ':')
                {
                    _hour = Int16.Parse(tmp);
                    tmp = String.Empty;
                    continue;
                }
                tmp += ch;
            }
            _minute = Int16.Parse(tmp);

            if (_hour >= 12 && _hour < 24)
                _hour -= 12;
            else if (_hour < 0 || _hour > 24 || _minute < 0 || _minute > 60)//дополнены условия определения неверного времени
                return "Time is wrong";
            else if (_hour == 24)//для 24:00
            {
                _hour = 0;
                _minute = 0;
            }

            //исправлены условия выбора формулы для рассчётов
            if (_minute * 6 < _hour * 30 + _minute * 0.5)
                return "Angle between arrows: " + (_hour * 30 + _minute * 0.5 - _minute * 6);//дополнен вывод на экран
            else if (_minute * 6 > _hour * 30 + _minute * 0.5)
                return "Angle between arrows: " + (_minute * 6 - (_hour * 30 + _minute * 0.5));//дополнен вывод на экран
            else return "Angle between arrows: " + 0;//дополнен вывод на экран
        }
    }
}
