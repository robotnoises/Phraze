using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Phraze
{
    /// <summary>
    /// Todo
    /// </summary>
    public class Phrase
    {
        private string _phrase;
        private HashSet<string> _phraseWords;
        private string _boundaryStart;
        private string _boundaryEnd;
                
        public Phrase(string input)
        {
            Init(input);
        }

        public bool Match(string matchText, double confidenceFloor = 0.7)
        {
            return CalculateMatchConfidence(matchText) >= confidenceFloor;
        }

        private void Init(string input)
        {
            _phrase = input;
                        
            if (!string.IsNullOrEmpty(_phrase))
            {
                var wordArray = _phrase.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
                _phraseWords = new HashSet<string>(wordArray);
                _boundaryStart = RemoveNonWordChars(wordArray[0]);
                _boundaryEnd = RemoveNonWordChars(wordArray[wordArray.Length - 1]);
            }
            else
            { 
                // TODO: throw Exception
            }
        }
        
        private double CalculateMatchConfidence(string matchText)
        {
            var confidence = 0.0;
            var phraseMatcher = new Fuzzy(_boundaryStart, _boundaryEnd);

            // If matcher can't find the boundary words, return with 0 confidence
            if (!phraseMatcher.IsMatch(matchText)) return 0.0;

            var matchScore = ScoreMatches(phraseMatcher.GetMatchedString(matchText));

            // temp 
            confidence = 1.0;

            return confidence;
        }

        private double ScoreMatches(string matchText)
        {
            var words = new HashSet<string>(matchText.Split(' ')); // Todo: better split
            var matchCount = 0.0; // Already matched boundary words
            var score = 0.0;
            var wordCount = (double)_phraseWords.Count;
            
            foreach (var word in words)
            {
                var matcher = new Fuzzy(word);
                
                if (matcher.IsMatch(_phrase))
                {
                    matchCount++;
                }
            }

            score = matchCount / wordCount;

            return score;
        }

        private string RemoveNonWordChars(string word)
        {
            return Regex.Replace(word, @"[\W]", "");
        }

        private char[] delimeters = 
        { 
            ' ', 
            ',', 
            ';', 
            '-', 
            '_', 
            '|', 
            '\\', 
            '/' 
        };
    }

    internal class Fuzzy
    {
        private Regex _matcher;
        // private static HashSet<string> matchSet; // not sure...

        // Does a "Fuzzy" match for a single word
        public Fuzzy(string input)
        {
            _matcher = new Regex(ToFuzzyWord(input), RegexOptions.IgnoreCase);
        }

        // Does a "Fuzzy" match from start point to end endpoint... i.e. a fuzzy phrase
        public Fuzzy(string start, string end)
        {
            _matcher = new Regex(string.Format("({0})([\\w\\s]+)({1})", ToFuzzyWord(start), ToFuzzyWord(end)), RegexOptions.IgnoreCase);
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

            if (numChars <= 3) return input;

            var firstChar = asCharArray[0];
            var lastChar = asCharArray[numChars - 1];
            var innerChars = RemoveEnds(asCharArray.ToArray());
            
            for (var i = 1; i < numChars - 1; i++)
            {
                var idx = i - 1;
                innerChars[idx] = asCharArray[i];
            }

            var charHashSet = new HashSet<char>(innerChars);
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
