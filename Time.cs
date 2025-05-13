using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCalcREFACTORED
{
    /// <summary>
    /// Время в диапазоне: [00:00; 23:59]
    /// </summary>
    public class Time
    {
        private int _hours;
        private int _minutes;

        public int Hours
        {
            get => _hours;
            set
            {
                _hours = value % 24;
                if (_hours < 0)
                {
                    _hours += 24;
                }
            }
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value % 60;
                Hours += value / 60;
                if (_minutes < 0)
                {
                    _minutes += 60;
                    Hours -= 1;
                }
            }
        }

        public Time(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
        }

        public Time(int minutes)
        {
            Minutes = minutes;
        }

        public override string ToString() => $"{_hours:D2}:{_minutes:D2}";

        // Арифметические:

        // Унарные:
        public static Time operator -(Time t)
        {
            return new(-t._hours, -t._minutes);
        }

        // Бинарыне:
        public static Time operator +(Time t1, Time t2)
        {
            return new Time(t1.Hours + t2.Hours, t1.Minutes + t2.Minutes);
        }
        public static Time operator +(Time left, int right)
        {
            return left + new Time(right);
        }
        public static Time operator +(int left, Time right)
        {
            return new Time(left) + right;
        }

        public static Time operator -(Time t1, Time t2)
        {
            return new Time(t1.Hours - t2.Hours, t1.Minutes - t2.Minutes);
        }
        public static Time operator -(int left, Time right)
        {
            return new Time(left) - right;
        }
        public static Time operator -(Time left, int right)
        {
            return left - new Time(right);
        }

        public static Time operator *(Time t1, int num)
        {
            return new Time(t1.Hours * num, t1.Minutes * num);
        }
        public static Time operator *(int num, Time t1)
        {
            return new Time(t1.Hours * num, t1.Minutes * num);
        }

        public static Time operator /(Time t1, int num)
        {
            return new Time((t1.Hours *60 + t1.Minutes) / num);
        }

        public static bool TryParse(string input, out Time result)
        {
            result = new(0,0);

            if (string.IsNullOrWhiteSpace(input))
                return false;

            bool minus = false;
            if (input[0] == '-')
            {
                input = input[1..];
                minus = true;
            }

            string[] parts = input.Trim().Split(':');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out int hours))
                return false;

            if (!int.TryParse(parts[1], out int minutes))
                return false;

            result = new Time(hours, minutes);

            if (minus)
            {
                result = -result;
            }

            return true;
        }
    }
}
