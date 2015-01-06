using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Phraze.Exceptions;
using Phraze.Utils;
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
            _phrase = input;

            if (!string.IsNullOrEmpty(_phrase))
            {
                var wordArray = _phrase.Split(Delimeters.All, StringSplitOptions.RemoveEmptyEntries);
                
                _phraseWords = new HashSet<string>(wordArray);
                _boundaryStart = RemoveNonWordChars(wordArray[0]);
                _boundaryEnd = RemoveNonWordChars(wordArray[wordArray.Length - 1]);
            }
            else
            {
                throw new EmptyPhrazeException("Phrase cannot be empty.");
            }
        }

        public bool FuzzyMatch(string matchText, double confidenceFloor = 0.7)
        {
            return CalculateMatchConfidence(matchText) >= confidenceFloor;
        }

        private double CalculateMatchConfidence(string matchText)
        {
            var phraseMatcher = new Fuzzy(_boundaryStart, _boundaryEnd);

            // If matcher can't find any phrase that matches, return with 0 confidence
            if (!phraseMatcher.IsMatch(matchText)) return 0.0;

            var matchedPhrase = phraseMatcher.GetMatchedString(matchText);
            var words = new HashSet<string>(matchedPhrase.Split(Delimeters.All, StringSplitOptions.RemoveEmptyEntries)); 
            var phraseWordCount = _phraseWords.Count;
            var matchTextWordCount = words.Count;
            var confidence = 0.0;
            var matchCount = 0.0;

            foreach (var word in words)
            {
                var matcher = new Fuzzy(word);

                if (matcher.IsMatch(_phrase))
                {
                    matchCount++;
                }
            }

            // Calculate the scores 
            var scores = new List<double>();

            scores.Add(matchCount / phraseWordCount);
            scores.Add(matchCount / matchTextWordCount);
            
            confidence = DoAverage(scores);

            return confidence;
        }
        
        private string RemoveNonWordChars(string word)
        {
            return Regex.Replace(word, @"[\W]", "");
        }

        private double DoAverage(ICollection<double> values)
        {
            return values.Sum() / values.Count;
        }
    }

    /// <summary>
    /// Todo
    /// </summary>
    public class PhraseCollection : List<Phrase>
    {
        public PhraseCollection(ICollection<string> phrases) 
        {
            foreach (var phrase in phrases)
            {
                this.Add(new Phrase(phrase));
            }
        }

        public bool HasMatch(string matchText)
        {
            var matchFound = false;

            while (!matchFound)
            {
                Parallel.ForEach(this, phrase =>
                {
                    if (phrase.FuzzyMatch(matchText))
                    {
                        matchFound = true;
                    }
                });
            }

            return matchFound;
            
            //foreach (var phraseMatcher in this)
            //{
            //    if (phraseMatcher.FuzzyMatch(matchText))
            //    {
            //        return true;
            //    }
            //}

            //return false;
        }

        public Phrase GetTopMatch(string matchText)
        {
            foreach (var phrase in this)
            {
                if (phrase.FuzzyMatch(matchText))
                {
                    return phrase;
                }
            }
            
            return null;
        }

        // Todo GetMatches
    }
}
