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
        private Fuzzy _phraseMatcher;

        public Phrase(string input)
        {
            _phrase = input;

            if (!string.IsNullOrEmpty(_phrase))
            {
                var wordArray = _phrase.Split(Delimeters.All, StringSplitOptions.RemoveEmptyEntries);
                
                _phraseWords = new HashSet<string>(wordArray);
                _boundaryStart = RemoveNonWordChars(wordArray[0]);
                _boundaryEnd = RemoveNonWordChars(wordArray[wordArray.Length - 1]);

                SetPhraseMatcher();
            }
            else
            {
                throw new EmptyPhrazeException("Phrase cannot be empty.");
            }
        }

        public bool IsMatch(string input, double confidenceFloor = 0.7)
        {
            var isMatch = false;

            if (!_phraseMatcher.HasMatch(input)) return isMatch;

            var fuzzyMatches = _phraseMatcher.GetMatchedStrings(input);
            var locker = new object();

            Parallel.ForEach(fuzzyMatches, match => 
            { 
                if (CalculateMatchConfidence(match) >= confidenceFloor)
                {
                    lock (locker) isMatch = true;
                }
            });

            return isMatch;
            
            // return CalculateMatchConfidence(matchText) >= confidenceFloor;
        }

        public ICollection<string> Matches(string input, double confidenceFloor = 0.7)
        {
            var matches = new HashSet<string>();

            if (!_phraseMatcher.HasMatch(input)) return matches;

            var fuzzyMatches = _phraseMatcher.GetMatchedStrings(input);
            var locker = new object();

            Parallel.ForEach(fuzzyMatches, match =>
            {
                if (CalculateMatchConfidence(match) >= confidenceFloor)
                {
                    lock (locker) matches.Add(match);
                }
            });

            return matches;
        }

        //public string GetTopMatchedString(string matchText, double confidenceFloor = 0.7)
        //{
        //    var matches = GetMatchedStrings(matchText);
        //    var withConfidence = new Dictionary<string, double>();
        //    var locker = new object();

        //    Parallel.ForEach(matches, match =>
        //    {
        //        lock (locker) withConfidence.Add(match, CalculateMatchConfidence(match));
        //    });

        //    var top = withConfidence.OrderByDescending(x => x.Value).Select(x => x.Key).Take(1);

        //    return top.FirstOrDefault();
        //}

        private void SetPhraseMatcher()
        {
            _phraseMatcher = _phraseMatcher ?? new Fuzzy(_boundaryStart, _boundaryEnd);
        }

        private double CalculateMatchConfidence(string matchedString)
        {
            // If matcher can't find any phrase that matches, return with 0 confidence
            // if (!_phraseMatcher.HasMatch(matchedString)) return 0.0;

            // Get the exact phrase that was matched
            // var matchedPhrases = _phraseMatcher.GetMatchedStrings(matchedString);
            // var topPhrase = matchedPhrases.FirstOrDefault(); // Todo, need to get the top match here

            // Create a word array from the target phrase
            var targetPhraseWords = _phrase.Split(Delimeters.All, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            // Create a HashSet of "Fuzzy" words from the list of target words
            var fuzzyWords = new HashSet<Fuzzy>(targetPhraseWords.Select(x => new Fuzzy(x)));

            // Count-up the matches
            var matchedWordsCount = 0.0;
            var locker = new object();

            Parallel.ForEach(fuzzyWords, fuzzyWord =>
            {
                if (fuzzyWord.HasMatch(matchedString))
                {
                    lock (locker) matchedWordsCount++;
                }
            });

            // Calculate the scores
            var scores = new List<double>();

            // How many of the input text words were matches? Ex: If 3 out of 4 words were matched, that would return a score of .75
            scores.Add(matchedWordsCount / targetPhraseWords.Count);

            // How many matches were there within the context of the targetphrase?
            scores.Add(matchedWordsCount / matchedString.Split(Delimeters.All, StringSplitOptions.RemoveEmptyEntries).Count());

            // Average the results            
            return DoAverage(scores);
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
        public PhraseCollection()
        { 
        
        }

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

            Parallel.ForEach(this, phrase =>
            {
                if (phrase.IsMatch(matchText))
                {
                    matchFound = true;
                }
            });
            
            return matchFound;
        }

        public ICollection<string> Matches(string matchText)
        {
            var matchedPhrases = new List<string>();
            var locker = new object();

            Parallel.ForEach(this, phrase => 
            {
                if (phrase.IsMatch(matchText))
                {
                    lock (locker)
                    {
                        matchedPhrases.AddRange(phrase.Matches(matchText));
                    }
                }
            });

            return matchedPhrases;
        }
    }
}
