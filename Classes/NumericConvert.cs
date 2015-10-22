using System;
using System.Collections.Generic;
using System.Linq;

namespace OSEmulator3
{
    class NumericConvert
    {
        public static int StringToRadix(string s, int radix)
        {
            try
            {
                return int.Parse(Convert.ToString(int.Parse(s), radix));
            }
            catch
            {
                return 0;
            }
        }

        public static int GetNum(char t)
        {
            switch (t)
            {
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
                default:
                    return int.Parse(t.ToString());
            }
        }
        public static long HexToLong(string s)
        {
            long sum = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int m = GetNum(s[i]);
                sum = 16 * sum + m;
            }
            return sum;
        }
        public static int HexToInt(string s)
        {
            int sum = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int m = GetNum(s[i]);
                sum = 16 * sum + m;
            }
            return sum;
        }
        public static int BinaryToInt(string s)
        {
            int sum = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int m = GetNum(s[i]);
                sum = sum * 2 + m;
            }
            return sum;
        }

        public static long BinaryToLong(string s)
        {
            long sum = 0;
            for (int i = 0; i < s.Length; i++)
            {
                int m = GetNum(s[i]);
                sum = sum * 2 + m;
            }
            return sum;
        }

        public static string HexToBinary(string s)
        {
            string bin = "";
            for (int i = 0; i < s.Length; i++)
            {
                bin += Convert.ToString(GetNum(s[i]), 2);
            }
            return bin;
        }
    }
}
