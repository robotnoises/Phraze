using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Resources = Phraze.Properties.Resources;

namespace Phraze.Utils
{
    public static class Synonyms
    {
        private static List<string[]> _synonyms;
        private static HashSet<string> _words;

        static Synonyms()
        {
            if (_words == null) GetSetWords();
        }

        public static IEnumerable<string> GetAll(string word)
        {
            var wordLower = word.ToLower();

            if (!HasSynonym(wordLower))
            { 
                var setOfOne = new HashSet<string>();
                setOfOne.Add(wordLower);
                return setOfOne;
            }

            var list = _synonyms.Where(x => x.Contains(wordLower));
            var flatList = new List<string>();
            var locker = new object();

            Parallel.ForEach(list, item =>
            {
                lock (locker)
                {
                    flatList.AddRange(item);
                }
            });

            return new HashSet<string>(flatList);
        }

        private static bool HasSynonym(string word)
        {
            return _words.Contains(word.ToLower());
        }

        private static void GetSetWords()
        {
            var resource = JsonConvert.DeserializeObject<Words>(Resources.synonyms.ToLower());
            _synonyms = resource.words.ToList();

            var listOfWords = new List<string>();

            foreach (var wordGroup in resource.words)
            {
                listOfWords.AddRange(wordGroup);
            }

            _words = new HashSet<string>(listOfWords.Select(x => x.ToLower()));
        }
    }

    internal class Words
    {
        public string name { get; set; }
        public string description { get; set; }
        public string[][] words { get; set; }
    }
}
