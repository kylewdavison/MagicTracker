using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MtgApiManager.Lib.Model;
using MtgApiManager.Lib.Service;

namespace MagicTracker.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCardSearch()
        {
            List<Card> listOfCards = new List<Card>();
            CardService service = new CardService();
            var result = service.Where(x => x.Name, "Lightning Bolt")
                //.Where(x => x.Set, "c17")
                .All();
            /*            foreach (var card in result.Value)
                        {
                            var printings = card.Printings;
                            foreach (var set in printings)
                            {
                                Console.WriteLine(set);
                            }

                        }*/

            if (result.Value.Count != 0)
            {
                foreach (var printing in result.Value[0].Printings)
                Console.WriteLine(printing);
            }
            //Console.ReadLine();

        }
    }
}
