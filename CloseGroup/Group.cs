using System.Collections.Generic;

namespace CloseGroup
{
    public class Group
    {
        public string Name { get; set; }

        public List<KeyWord> KeyWords { get; set; }

        public List<string> Products { get; set; }
    }

    public class KeyWord
    {
        public string Token { get; set; }

        public int Weight { get; set; }
    }
}
