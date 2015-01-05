using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phraze;

namespace PhrazeTests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void MatchBoundariesExact()
        {
            var phrase = "This is a medium-sized test phrase";
            
            var matchPhrase1 = "Thos is a uh is phrase.";
            var matchPhrase2 = "Will not match anything.";

            var p = new Phrase(phrase);

            var goodMatch = p.Match(matchPhrase1);
            var badMatch = p.Match(matchPhrase2);
            var empty = p.Match("");

            Assert.IsTrue(goodMatch);
            Assert.IsFalse(badMatch);
            Assert.IsFalse(empty);
        }
    }
}
