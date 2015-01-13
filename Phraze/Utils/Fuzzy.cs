using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Phraze.Utils
{
    internal class Fuzzy
    {
        private Regex _matcher;
        
        // Does a "Fuzzy" match for a single word
        public Fuzzy(string input)
        {
            _matcher = new Regex(input.ToFuzzyWord(), RegexOptions.IgnoreCase);
        }

        // Does a "Fuzzy" match from start point to end endpoint... i.e. a fuzzy phrase
        public Fuzzy(string start, string end)
        {
            _matcher = new Regex(
                string.Format("({0})({1})({2})", 
                start.ToFuzzyWord(FuzzyWordOptions.MatchExact), 
                @"(...*?)", // Non-greedy with minimum of three chars between boundary words
                end.ToFuzzyWord(FuzzyWordOptions.MatchExact)), 
                RegexOptions.IgnoreCase);
        }

        // Checks to see if there is a matched word or phrase
        public bool HasMatch(string textToMatch)
        {
            return _matcher.IsMatch(textToMatch.RemovePunctuation());
        }
                
        public ICollection<string> GetMatchedStrings(string textToMatch)
        {
            return _matcher.Matches(textToMatch.RemovePunctuation()).Cast<Match>().Select(x => x.Value).ToList();
        }
    }

    internal static class StringExtensions
    {
        public static string ToFuzzyWord(this string input, FuzzyWordOptions option = FuzzyWordOptions.MatchFuzzy)
        {
            var wordList = new List<string>(); 
            var locker = new object();
            var fuzzyWord = new StringBuilder(1000);

            if (option != FuzzyWordOptions.NoSynonyms)
            {
                wordList.AddRange(Synonyms.GetAll(input));
            }
            else 
            {
                wordList.Add(input);
            }

            Parallel.ForEach(wordList, word =>
            {
                var asCharArray = word.RemovePunctuation().ToCharArray();
                var numChars = asCharArray.Length;

                if (numChars <= 3 || option == FuzzyWordOptions.MatchExact)
                {
                    lock (locker)
                    {
                        fuzzyWord.Append(@"\b" + word + @"\b|");
                        return; // Break-out of this thread
                    }
                }

                var firstChar = asCharArray[0];
                var lastChar = asCharArray[numChars - 1];
                var innerChars = asCharArray.ToArray().RemoveEnds();

                for (var i = 1; i < numChars - 1; i++)
                {
                    var idx = i - 1;
                    innerChars[idx] = asCharArray[i];
                }

                var charHashSet = new HashSet<char>(innerChars); // Note: Removes duplicate chars
                var innerCharsAsString = new string(charHashSet.ToArray());

                lock (locker)
                {
                    fuzzyWord.Append(@"\b" + firstChar);
                    fuzzyWord.Append(string.Format("[{0}]", innerCharsAsString));
                    fuzzyWord.Append("{" + (innerChars.Length - 1).ToString() + ",");
                    fuzzyWord.Append((innerChars.Length).ToString() + "}");
                    fuzzyWord.Append(@"[\w]{0,1}");
                    fuzzyWord.Append(lastChar + @"\b|");
                }
            });

            return fuzzyWord.ToString().TrimEndOrOperator();
        }

        public static string TrimEndOrOperator(this string input)
        {
            char[] charsToTrim = { ' ', '|' };

            return input.TrimEnd(charsToTrim);
        }

        // TODO: make an interface for the following

        public static string[] RemoveEnds(this string[] array)
        {
            if (array.Length <= 2) return array;

            var numItems = array.Length;
            var newArray = new string[numItems - 2];

            for (var i = 1; i < numItems - 1; i++)
            {
                var idx = i - 1;
                newArray[idx] = array[i];
            }

            return newArray;
        }

        public static char[] RemoveEnds(this char[] array)
        {
            if (array.Length <= 2) return array;

            var numItems = array.Length;
            var newArray = new char[numItems - 2];

            for (var i = 1; i < numItems - 1; i++)
            {
                var idx = i - 1;
                newArray[idx] = array[i];
            }

            return newArray;
        }

        public static string RemovePunctuation(this string input)
        {
            return Regex.Replace(input, @"[^\w\s]", "");
        }
    }

    [Flags]
    internal enum FuzzyWordOptions
    {
        MatchFuzzy,
        NoSynonyms,
        MatchExact
    }
    
}
