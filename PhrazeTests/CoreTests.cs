using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Phraze;
using Phraze.Utils;

namespace PhrazeTests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void DoFuzzyMatch_ValidData()
        {
            var targetPhrase = "This is a medium-sized test phrase"; // This is what the application is configured to look for
            var matchText1 = "Thos is a uh medium phrase."; // This is an input phrase to test against the targetPhrase
            var matchText2 = "Will not match anything."; // This is an input phrase to test against the targetPhrase

            var p = new Phrase(targetPhrase);

            var goodMatch = p.FuzzyMatch(matchText1);
            var badMatch = p.FuzzyMatch(matchText2);
            var empty = p.FuzzyMatch("");

            Assert.IsTrue(goodMatch);
            Assert.IsFalse(badMatch);
            Assert.IsFalse(empty);
        }

        [TestMethod]
        public void Test1()
        {
            var targetPhrase = "If ... wins, I will";
            var matchText = "Friendly bet - If Alabama wins, I will give complete control on my account to the lucky winner until the SEC championship (one week) to do with as they please (comment, message, add/drop subs, change flairs, whatever). If Auburn wins, I want either the same or something equally delightful. Who is game?";
            
            var p = new Phrase(targetPhrase);

            var isMatch = p.FuzzyMatch(matchText);

            Assert.IsTrue(isMatch);
        }

        [TestMethod]
        public void Test2()
        {
            var targetPhrase = "If ... wins, I will";
            var matchPhrase = "This is a nonsense phrase containing all of the above words, for instance: foo bar wins if foo bar baz I will";

            var p = new Phrase(targetPhrase);

            var isMatch = p.FuzzyMatch(matchPhrase);

            Assert.IsFalse(isMatch);
        }

        [TestMethod]
        public void Test3()
        {
            var matchText = "one two four";
            var targetPhrases = new List<string>();
            
            targetPhrases.Add("one two three four");
            targetPhrases.Add("dsfklsdjlkjss ddd");
            targetPhrases.Add("dsf gdf");
            targetPhrases.Add("sdfs");
            targetPhrases.Add("sdfsdfsd dsfs");

            var phrazes = new PhraseCollection(targetPhrases);

            Assert.IsTrue(phrazes.HasMatch(matchText));
        }

        [TestMethod]
        public void Test4()
        {
            var matchText = "[â€“]LSU TigersYesh 14 points15 points16 points 2 hours ago (2 children)I want to be part of the mercenary QB trend.\n\nC'mon, Braxton. Come to LSU and lead us to a championship. I'd even stop hating on OSU if you'd just come to BR for a year. \n\n</form>permalink";
            var targetPhrase = "If ... loses, you";

            var matcher = new Phrase(targetPhrase);

            Assert.IsFalse(matcher.FuzzyMatch(matchText));
        }

        [TestMethod]
        public void GetSynonyms()
        {
            var word = "y'all";

            var listOfSynonyms = Synonyms.GetSynonyms(word);

            Assert.IsNotNull(listOfSynonyms);
        }
    }
}
