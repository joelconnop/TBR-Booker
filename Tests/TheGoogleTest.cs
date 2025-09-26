using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBRBooker.Base;
using TBRBooker.Business;

namespace Tests
{
    [TestClass]
    public class TheGoogleTest
    {

        [TestMethod]
        public void ConnectAndDisplay()
        {
            EnsureSettings();

            var result = TheGoogle.GetGoogleCalendar(
                DateTime.Now, DateTime.Now.AddMonths(1), true);

            result.ForEach(x => Console.WriteLine(x));
        }

        [TestMethod]
        public void PlacesSearch_WithoutSessionToken_ReturnsLocalResults()
        {
            EnsureSettings();

            var results = TheGoogle.PlacesSearch("Nerang QLD");

            Assert.IsNotNull(results, "Places search returned null.");
            Assert.IsTrue(results.Length > 0, "Places search should return at least one suggestion.");
            Assert.IsTrue(results.Any(r => r.Contains("Nerang")), "Expected Nerang to appear in the suggestions.");
        }

        [TestMethod]
        public void PlacesSearch_WithSessionToken_ReturnsConsistentResultsWithinSession()
        {
            EnsureSettings();

            var searchTerm = "Southport QLD";
            var sessionToken = Guid.NewGuid().ToString();

            var firstResults = TheGoogle.PlacesSearch(searchTerm, sessionToken);
            Assert.IsNotNull(firstResults, "Initial places search returned null.");
            Assert.IsTrue(firstResults.Length > 0, "Initial places search should return at least one suggestion.");

            var secondResults = TheGoogle.PlacesSearch(searchTerm, sessionToken);
            Assert.IsNotNull(secondResults, "Repeat places search returned null.");
            Assert.IsTrue(secondResults.Length > 0, "Repeat places search should return at least one suggestion.");
            Assert.IsTrue(secondResults.Contains(firstResults[0]), "Expected repeat search with same session token to include the initial top suggestion.");
        }

        private static void EnsureSettings()
        {
            if (Settings.Inst() == null)
            {
                Settings.CreateDefaultInst();
            }

            if (string.IsNullOrEmpty(Settings.Inst().GoogleAPIKey))
            {
                Assert.Inconclusive("Google API key is not configured for integration tests.");
            }
        }
    }
}
