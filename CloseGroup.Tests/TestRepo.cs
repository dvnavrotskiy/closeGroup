using System.Collections.Generic;

namespace CloseGroup.Tests
{
    internal class TestRepo : IRepo
    {
        private readonly List<Group> groups = new List<Group>
        {
            new Group
            {
                Name = "Шоколадные батончики",
                Products = new List<string>
                {
                    "шоколадный батончик сникерс",
                    "шокаладный батончик марс",
                    "шоколадный батончик баунти",
                    "шоколадный батончик твикс",
                    "шоколадный батончик степ"
                }
            },
            new Group
            {
                Name = "Стиральные порошки",
                Products = new List<string>
                {
                    "порошок стиральный тайд",
                    "стиральный порошок персил",
                    "стиральный порошок ариэль",
                    "стиральный порошок ласка"
                }
            }
        };

        public string GetGroupByExactProductName(string productName)
        {
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
        }

        public IList<Group> List()
        {
            return groups;
        }

        public void AddProduct(Group group, string productName)
        {
            group.Products.Add(productName);
        }
    }
}
