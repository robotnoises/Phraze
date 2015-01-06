using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Resources = Phraze.Properties.Resources;

namespace Phraze.Utils
{
    internal static class Synonyms
    {
        private static List<string[]> _synonyms;
        private static HashSet<string> _words;

        static Synonyms()
        {
            if (_words == null) GetWords();
        }

        public static void Foo()
        {
            var temp = 1 + 1;
        }

        private static void GetWords()
        {
            var resource = JsonConvert.DeserializeObject<Words>(Resources.wordConversions);
            _synonyms = resource.words.ToList();

            var listOfWords = new List<string>();

            foreach (var wordGroup in resource.words)
            {
                listOfWords.AddRange(wordGroup);
            }

            _words = new HashSet<string>(listOfWords);
        }
    }

    internal class Words
    {
        public string name { get; set; }
        public string description { get; set; }
        public string[][] words { get; set; }
    }
}
