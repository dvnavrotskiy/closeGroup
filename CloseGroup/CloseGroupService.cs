using System;
using System.Collections.Generic;
using System.Linq;

namespace CloseGroup
{
    public interface ICloseGroupService
    {
        /// <summary>
        /// Поиск ближайшей группы по названию продукта
        /// </summary>
        /// <param name="productName">продукт</param>
        /// <returns>имя группы</returns>
        string CloseGroupFor(string productName);
        
        /// <summary>
        /// Анализ групп для выявления ключевых слов
        /// </summary>
        void AnalyzeGroups();

        /// <summary>
        /// Информация о группах
        /// </summary>
        /// <returns>текст</returns>
        string GroupsInfo();
    }

    public class CloseGroupService : ICloseGroupService
    {
        private readonly IRepo repo;
        private readonly IProductNameProcessor productNameProcessor;
        private readonly IKeyWordAnalyzer analyzer;
        private readonly double wordSimilarityRatio;
        private readonly bool needAddNewProducts;

        public CloseGroupService(ISettings settings, IRepo repo, IProductNameProcessor productNameProcessor, IKeyWordAnalyzer analyzer)
        {
            this.repo = repo;
            this.productNameProcessor = productNameProcessor;
            this.analyzer = analyzer;
            wordSimilarityRatio = settings.Get<double>("WordSimilarityRatio");
            needAddNewProducts = settings.Get<bool>("NeedAddNewProducts");
            // анализ групп на старте
            // в проде так быть не должно
            analyzer.Analyze(false);
        }

        public string CloseGroupFor(string productName)
        {
            var groupName = repo.GetGroupByExactProductName(productName);
            if (groupName != null)
                return groupName;

            var words = productNameProcessor.Process(productName);
            var groupMap = new Dictionary<Group, int>();
            foreach (var group in repo.List())
            {
                groupMap[group] = CalcProductWeight(group, words);
            }

            var best = groupMap.First();
            foreach (var pair in groupMap.Skip(1))
            {
                if (best.Value < pair.Value)
                    best = pair;
            }

            if (best.Value > 0)
            {
                if (needAddNewProducts)
                    repo.AddProduct(best.Key, productName);
                return best.Key.Name;
            }

            throw new Exception($"Не удалось определить группу для {productName}");
        }

        public void AnalyzeGroups()
        {
            analyzer.Analyze(true);
        }

        public string GroupsInfo()
        {
            return string.Join(
                "\n",
                repo.List()
                    .Select(x => $"{x.Name}: {x.KeyWords.Count} ключевых слов на {x.Products.Count} продуктах")
            );
        }

        private int CalcProductWeight(Group group, IList<string> productWords)
        {
            var result = 0;
            var map = group.KeyWords.ToDictionary(x => x.Token, x => x.Weight);

            foreach (var word in productWords)
            {
                if (map.TryGetValue(word, out var weight))
                {
                    result += weight;
                    continue;
                }

                foreach (var key in map.Keys.ToArray())
                {
                    if (!WordUtils.IsWordsSimilar(key, word, wordSimilarityRatio))
                        continue;
                    result += map[key];
                    break;
                }
            }

            return result;
        }
    }
}
