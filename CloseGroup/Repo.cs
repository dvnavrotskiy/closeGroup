using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace CloseGroup
{
    public interface IRepo
    {
        string GetGroupByExactProductName(string productName);
        void WriteDown();
        IList<Group> List();
        void AddProduct(Group bestKey, string productName);
    }

    public class Repo : IRepo
    {
        private readonly List<Group> groups;
        private readonly string path;

        public Repo(ISettings settings)
        {
            path = settings.Get<string>("RepoPath");
            using (var file = File.OpenText(path))
            using (var reader = new JsonTextReader(file))
            {
                var serializer = JsonSerializer.Create();
                groups = serializer.Deserialize<List<Group>>(reader);
            }
        }

        /// <summary>
        /// Поиск группы по точному совпадению названия продукта
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public string GetGroupByExactProductName(string productName)
        {
            // поиск по точному соответствию имени лучше возлагать на SQL - при наличии индекса это будет очень быстро
            // сигнатура метода будет той же, внутри обращение в БД
            foreach (var group in groups)
            {
                foreach (var product in group.Products)
                {
                    if (product == productName)
                    {
                        return group.Name;
                    }
                }
            }

            return null;
        }

        public void WriteDown()
        {
            var text = JsonConvert.SerializeObject(groups, Formatting.Indented);
            File.WriteAllText(path, text);
        }

        public IList<Group> List()
        {
            return groups;
        }

        public void AddProduct(Group group, string productName)
        {
            group.Products.Add(productName);
            WriteDown();
        }
    }
}
