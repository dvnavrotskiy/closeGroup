using System.Collections.Generic;
using System.Linq;

namespace CloseGroup
{
    /// <summary>
    /// Анализ групп
    /// </summary>
    public interface IKeyWordAnalyzer
    {
        void Analyze(bool forced);
    }

    public class KeyWordAnalyzer : IKeyWordAnalyzer
    {
        private readonly IRepo repo;
        private readonly IProductNameProcessor productNameProcessor;
        private readonly double wordSimilarityRatio;

        public KeyWordAnalyzer(IRepo repo, IProductNameProcessor productNameProcessor, ISettings settings)
        {
            this.repo = repo;
            this.productNameProcessor = productNameProcessor;

            wordSimilarityRatio = settings.Get<double>("WordSimilarityRatio");
        }

        public void Analyze(bool forced)
        {
            var analyzed = false;
            var groups = repo.List();
            foreach (var group in groups)
            {
                if (!forced && group.KeyWords != null && group.KeyWords.Any())
                    continue;

                AnalyzeGroup(group, groups);
                analyzed = true;
            }

            if (analyzed)
                repo.WriteDown();
        }

        private void AnalyzeGroup(Group group, IList<Group> groups)
        {
            var map = new Dictionary<string, int>();
            foreach (var product in group.Products)
                AnalyzeProduct(map, product);

            group.KeyWords = map
                .Where(x => x.Value > 1)
                .Select(x => new KeyWord {Token = x.Key, Weight = NormalizeWeignt(x.Value, group.Products.Count)})
                .ToList();

            // сравнение со словами в дургих гурппах
            foreach (var otherGroup in groups)
            {
                if (otherGroup.Name == group.Name)
                    return;

                var otherWords = otherGroup.KeyWords.ToList();

                foreach (var word in otherWords)
                {
                    var keyWord = group.KeyWords.FirstOrDefault(x => x.Token == word.Token);
                    if (keyWord == null)
                        continue;

                    // при совпадении уменьшить вес или удалить
                    if (keyWord.Weight > word.Weight)
                    {
                        keyWord.Weight -= word.Weight;
                        otherGroup.KeyWords.Remove(word);
                    }
                    else if (keyWord.Weight < word.Weight)
                    {
                        word.Weight -= keyWord.Weight;
                        group.KeyWords.Remove(word);
                    }
                    else
                    {
                        otherGroup.KeyWords.Remove(word);
                        group.KeyWords.Remove(word);
                    }
                }
            }
        }

        private void AnalyzeProduct(Dictionary<string, int> map, string product)
        {
            var words = productNameProcessor.Process(product);
            foreach (var word in words)
            {
                if (map.TryGetValue(word, out var cnt))
                {
                    map[word] = cnt + 1;
                    continue;
                }

                var foundSimilar = false;
                foreach (var key in map.Keys.ToArray())
                {
                    if (!WordUtils.IsWordsSimilar(key, word, wordSimilarityRatio))
                        continue;
                    map[key] = map[key] + 1;
                    foundSimilar = true;
                    break;
                }
                if (foundSimilar)
                    continue;

                map[word] = 1;
            }
        }

        private static int NormalizeWeignt(int wordCount, int productCount)
        {
            return 100 * wordCount / productCount;
        }
    }
}