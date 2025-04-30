using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloseGroup
{
    /// <summary>
    /// Обработчик названий
    /// </summary>
    public interface IProductNameProcessor
    {
        /// <summary>
        /// Формирует список значащих слов для названия продукта
        /// </summary>
        /// <param name="productName">название продукта</param>
        /// <returns>список слов</returns>
        IList<string> Process(string productName);
    }

    public class ProductNameProcessor : IProductNameProcessor
    {
        private const char SplitChar = ' ';
        private readonly char[] skipChars = {',', '.', '-', ':', ';'};

        private readonly Dictionary<char, char> replaceMap =
            new Dictionary<char, char>
            {
                {'ё', 'е'}
            };

        public IList<string> Process(string productName)
        {
            var split = productName.ToLower().Split(SplitChar);
            var result = new List<string>();
            foreach (var token in split)
            {
                if (string.IsNullOrWhiteSpace(token))
                    continue;

                // если слово начато с цифр, то это, скорее всего, вес и он не нужен
                if (token[0] > '0' && token[0] < '9')
                    continue;

                var word = NormalizeWord(token);

                // короткие слова плохи для категоризации, это либо предлоги, либо слишком смелые сокращения
                if (word.Length < 3)
                    continue;

                if (result.Contains(word))
                    continue;

                result.Add(word);
            }

            return result;
        }

        // Нормализует слово - выкидывает мусор, заменяет буквы и т.п.
        private string NormalizeWord(string token)
        {
            var word = new StringBuilder(token.Length);

            foreach (var c in token)
            {
                if (skipChars.Contains(c))
                    continue;

                if (replaceMap.TryGetValue(c, out var rc))
                {
                    word.Append(rc);
                    continue;
                }

                word.Append(c);
            }

            return word.ToString();
        }
    }
}
