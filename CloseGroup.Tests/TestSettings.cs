using System.Collections.Generic;

namespace CloseGroup.Tests
{
    internal class TestSettings : ISettings
    {
        private readonly Dictionary<string, object> dic = new Dictionary<string, object>
        {
            { "WordSimilarityRatio", 0.2},
            { "NeedAddNewProducts",  true}
        };

        public T Get<T>(string key)
        {
            return (T)dic[key];
        }
    }
}
