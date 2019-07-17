using System;
using System.Collections.Generic;
using System.Linq;
using MagicTracker.Data;
using MagicTracker.Models;
using MagicTracker.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mtgCard = MtgApiManager.Lib.Model;
using mtgService = MtgApiManager.Lib.Service;

namespace MagicTracker.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public Guid _userId = new Guid("5f0f4dfa-3ba8-4eb2-96c6-6150a278392e");

        [TestMethod]
        public void TestCardSearch()
        {
            List<mtgCard.Card> listOfResults = new List<mtgCard.Card>();
            mtgService.CardService apiService = new mtgService.CardService();
            var result = apiService
                .Where(x => x.Name, "Huntmaster of the Fells|Reliquary Tower|Snapcaster Mage")
                .Where(x => x.OrderBy, "name")
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
                foreach (var card in result.Value)
                Console.WriteLine(card.Name);
            }
            //Console.ReadLine();

        }
/*
        [TestMethod]
        public void GetCollection()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                    .Cards
                    .Where(e => e.OwnerId == _userId)
                    .Select(
                        e =>
                            new CollectionItem
                            {
                                CardId = e.CardId,
                                CardName = e.Name
                            }
                    );
                var results = query.ToArray();
            }
        }*/
    }
}
