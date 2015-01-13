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
            var matchText1 = "This is a uh meudium phrase."; // This is an input phrase to test against the targetPhrase
            var matchText2 = "Will not match anything."; // This is an input phrase to test against the targetPhrase

            var p = new Phrase(targetPhrase);

            var goodMatch = p.IsMatch(matchText1);
            var badMatch = p.IsMatch(matchText2);
            var empty = p.IsMatch("");

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

            var isMatch = p.IsMatch(matchText);
            var matches = p.Matches(matchText);
            Assert.IsTrue(isMatch);
        }

        [TestMethod]
        public void WontMatchWordsOutOfOrder()
        {
            var targetPhrase = "If ... wins, I will";
            var matchPhrase = "This is a nonsense phrase containing all of the above words, for instance: foo bar wins if foo bar baz I will";

            var p = new Phrase(targetPhrase);

            var isMatch = p.IsMatch(matchPhrase);

            Assert.IsFalse(isMatch);
        }

        [TestMethod]
        public void FuzzyMatch()
        {
            var matchText = "WHERE ARE YOU /U/CLAUDELEMIEUX?!? You promised me a bet. Don't back down now. It's a straight up bet. No point spreads. No parlays. If Ohio State wins, you have to make a video of yourself singing the Buckeye Battle Cry and post it to /r/cfb along with a brief history of Ohio State Football. If Michigan wins, I'll do the same for your dumb school/fight song. And Ill change my flair since you mentioned that. Do we have a deal?";
            var targetPhrase = "If ... wins, you have to";

            var matcher = new Phrase(targetPhrase);
            var isMatch = matcher.IsMatch(matchText);
            
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
            var isMatch = matcher.IsMatch(matchText);

            Assert.IsTrue(isMatch);
        }

        [TestMethod]
        public void Temp2()
        {
            var list = new List<string>();
            var matchText = @"If this were the BCS era, we'd still get this game, but it'd be the Rose Bowl. Pretty crazy.";
            
            var t1 = "If ... win, I'll";
            var t2 = "Anyone wanna bet";
            
            list.Add(t1);
            list.Add(t2);
            
            var matcher = new PhraseCollection(list);
            var isMatch = matcher.HasMatch(matchText);
            var matches = matcher.Matches(matchText);

            Assert.IsFalse(isMatch);
            Assert.IsFalse(matches.Count > 0);
        }

        /* False positives */

        /*
            I tried smoke recently and it was fantastic, but a little pricey. It has a really cool view of downtown Dallas.
            Also you can find Cane Rosso in both Dallas and Fort Worth which is really good pizza.
            Edit: If you go to cane Rosso the honey bastard isn't on their menu, but it's amazing I would look into it.
         */

        /*
            Never bet against Boise in the Fiesta Bowl.
         */

        /*
            It only makes zero sense if you have no respect for the atmosphere and culture BSU has built around its football program.
            Harsin, Yates, and Stanford built a monster of a team in just one year, and it improved by freakish leaps and bounds over the course of the season. The BSU team that beat Arizona at the end of the year was so much better than the team that debuted against Ole Miss.
            Why leave a program you know and respect just to make a lateral move career wise? You can argue that the prestige of being the OC at Ohio might tempt him, but he would still just be an OC. Hell, in a year or two he will be considered for head coaching jobs if he continues his success at BSU.
         */
    }
}
