using System;

namespace CloseGroup
{
    public class WordUtils
    {
        // сравнение двух слов с некоторой погрешностью
        public static bool IsWordsSimilar(string a, string b, double ratio)
        {
            if (ratio < 0)
                ratio = 0;
            else if (ratio > 1)
                ratio = 1;

            var delta = (int)(a.Length * ratio); // сколько букв можно записать в допустимое отличие
            var ldelta = Math.Abs(a.Length - b.Length);

            if (ldelta > delta)
                return false;

            for (var i = 0; i < Math.Min(a.Length, b.Length); ++i)
            {
                if (a[i] == b[i])
                    continue;
                if (++ldelta > delta)
                    return false;
            }

            return true;
        }
    }
}