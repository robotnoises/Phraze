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
            if (!phraseMatcher.HasMatch(matchText)) return 0.0;

            // Get the exact phrase that was matched
            var matchedPhrase = phraseMatcher.GetMatchedString(matchText);

            // Create a word array from the target phrase
            var targetPhraseWords = _phrase.Split(Delimeters.All, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            // Create a HashSet of "Fuzzy" words from the list of tart words
            var fuzzyWords = new HashSet<Fuzzy>(targetPhraseWords.Select(x => new Fuzzy(x)));

            // Count-up the matches
            var matchedWordsCount = 0.0;
            var locker = new object();

            Parallel.ForEach(fuzzyWords, fuzzyWord =>
            {
                if (fuzzyWord.HasMatch(matchedPhrase))
                {
                    lock (locker) matchedWordsCount++;
                }
            });

            // Calculate the scores
            var scores = new List<double>();

            // How many of the input text words were matches? Ex: If 3 out of 4 words were matched, that would return a score of .75
            scores.Add(matchedWordsCount / targetPhraseWords.Count);

            // How many matches were there within the context of the targetphrase?
            scores.Add(matchedWordsCount / matchedPhrase.Split(Delimeters.All, StringSplitOptions.RemoveEmptyEntries).Count());

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
                if (phrase.FuzzyMatch(matchText))
                {
                    matchFound = true;
                }
            });
            
            return matchFound;
        }

        public PhraseCollection GetMatches(string matchText)
        {
            var matchedPhrases = new PhraseCollection();
            var locker = new object();

            Parallel.ForEach(this, phrase => 
            {
                if (phrase.FuzzyMatch(matchText))
                {
                    lock (locker)
                    {
                        matchedPhrases.Add(phrase);
                    }
                }
            });

            return matchedPhrases;
        }
    }
}
