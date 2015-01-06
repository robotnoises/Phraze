using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Phraze.Utils
{
    internal class Fuzzy
    {
        private Regex _matcher;
        
        // Does a "Fuzzy" match for a single word
        public Fuzzy(string input)
        {
            _matcher = new Regex(ToFuzzyWord(input), RegexOptions.IgnoreCase);
        }

        // Does a "Fuzzy" match from start point to end endpoint... i.e. a fuzzy phrase
        public Fuzzy(string start, string end)
        {
            _matcher = new Regex(string.Format("({0})({1})({2})", ToFuzzyWord(start), @"[\w\W]{3,}", ToFuzzyWord(end)), RegexOptions.IgnoreCase);
        }

        // Checks to see if there is a matched word or phrase
        public bool IsMatch(string textToMatch)
        {
            return _matcher.IsMatch(textToMatch);
        }

        // TODO: Returns only the matched string
        public string GetMatchedString(string textToMatch)
        {
            var matches = _matcher.Matches(textToMatch).Cast<Match>().Select(x => x.Value).ToList();

            if (matches.Count <= 1) return matches.FirstOrDefault();

            // Todo, if there are multiple matches, return the highest scoring match
            return matches.FirstOrDefault();
        }

        private static string ToFuzzyWord(string input)
        {
            var asCharArray = input.ToCharArray();
            var numChars = asCharArray.Length;

            if (numChars <= 3) return @"\b" + input + @"\b";

            var firstChar = asCharArray[0];
            var lastChar = asCharArray[numChars - 1];
            var innerChars = RemoveEnds(asCharArray.ToArray());

            for (var i = 1; i < numChars - 1; i++)
            {
                var idx = i - 1;
                innerChars[idx] = asCharArray[i];
            }

            var charHashSet = new HashSet<char>(innerChars); // Removes duplicate chars
            var innerCharsAsString = new string(charHashSet.ToArray());

            var pattern = new StringBuilder(100);

            pattern.Append(@"\b" + firstChar);
            pattern.Append(string.Format("[{0}]", innerCharsAsString));
            pattern.Append("{" + (innerChars.Length - 1).ToString() + ",");
            pattern.Append((innerChars.Length).ToString() + "}");
            pattern.Append(@"[\w]{0,1}");
            pattern.Append(lastChar + @"\b");

            return pattern.ToString();
        }

        // TODO: make an interface for the following

        private static string[] RemoveEnds(string[] array)
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

        private static char[] RemoveEnds(char[] array)
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
    }
}
