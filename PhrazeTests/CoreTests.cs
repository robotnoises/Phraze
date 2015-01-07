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
        public void BasicMatch()
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
        public void ExactMatch()
        {
            var targetPhrase = "If ... wins, I will";
            var matchText = "Friendly bet - If Alabama wins, I will give complete control on my account to the lucky winner until the SEC championship (one week) to do with as they please (comment, message, add/drop subs, change flairs, whatever). If Auburn wins, I want either the same or something equally delightful. Who is game?";
            
            var p = new Phrase(targetPhrase);

            var isMatch = p.FuzzyMatch(matchText);

            Assert.IsTrue(isMatch);
        }

        [TestMethod]
        public void WontMatchWordsOutOfOrder()
        {
            var targetPhrase = "If ... wins, I will";
            var matchPhrase = "This is a nonsense phrase containing all of the above words, for instance: foo bar wins if foo bar baz I will";

            var p = new Phrase(targetPhrase);

            var isMatch = p.FuzzyMatch(matchPhrase);

            Assert.IsFalse(isMatch);
        }

        [TestMethod]
        public void FuzzyMatch()
        {
            var matchText = "WHERE ARE YOU /U/CLAUDELEMIEUX?!? You promised me a bet. Don't back down now. It's a straight up bet. No point spreads. No parlays. If Ohio State wins, you have to make a video of yourself singing the Buckeye Battle Cry and post it to /r/cfb along with a brief history of Ohio State Football. If Michigan wins, I'll do the same for your dumb school/fight song. And Ill change my flair since you mentioned that. Do we have a deal?";
            var targetPhrase = "If ... wins, you must";

            var matcher = new Phrase(targetPhrase);
            var isMatch = matcher.FuzzyMatch(matchText);
            
            Assert.IsTrue(isMatch);
        }

        [TestMethod]
        public void GetSynonyms()
        {
            var word = "y'all";

            var listOfSynonyms = Synonyms.GetAll(word);

            Assert.IsNotNull(listOfSynonyms);
        }

        [TestMethod]
        public void Temp()
        {

            var matchText = "If the filthy cougs beat us. I'll change flair for the whole off season, burn 3 huskie shirts on camera while wearing a coug shirt chanting Huck the fuskies. Provided a coug makes the same guarantee.";
            var targetPhrase = "If ... lose, I'll";

            var matcher = new Phrase(targetPhrase);
            var isMatch = matcher.FuzzyMatch(matchText);

            Assert.IsTrue(isMatch);
        }
    }
}
