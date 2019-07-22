using MagicTracker.Data;
using MagicTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Card = MagicTracker.Data.Card;
using MagicTracker.Models.Deck;
using System.Text.RegularExpressions;
using mtgCard = MtgApiManager.Lib.Model;
using mtgService = MtgApiManager.Lib.Service;
using MagicTracker.Models.CardApi;
using Newtonsoft.Json;

namespace MagicTracker.Services
{
    public class DeckService
    {
        private readonly Guid _userId;
        public int CreateDeck(DeckCreate model)
        {
            if (model.CardListString == null) { return -1; }
            List<string> tempCardList = new List<string>();
            List<string> newCardsForApi = new List<string>();
            Dictionary<string, int> tempCardDict = new Dictionary<string, int>();
            List<int> deckList = new List<int>();

            string[] tempCardNames = model.CardListString.Split('\n');
            tempCardNames = (from c in tempCardNames
                             select c.Trim()).ToArray();

            foreach (var card in tempCardNames)
            {
                if (card == "") { continue; }
                if (Char.IsDigit(card[0]) && (Char.IsDigit(card[1]) == false))
                {
                    int count = int.Parse(Char.ToString(card[0]));
                    var reducedCard = card.Remove(0, 1);
                    var finalCard = reducedCard.Trim();
                    tempCardList.Add(finalCard);
                    tempCardDict.Add(finalCard, count);
                }
                else if (Char.IsDigit(card[0]) && (Char.IsDigit(card[1])))
                {
                    int count = int.Parse(Char.ToString(card[0]) + Char.ToString(card[1]));
                    var reducedCard = card.Remove(0, 2);
                    var finalCard = reducedCard.Trim();
                    tempCardList.Add(finalCard);
                    tempCardDict.Add(finalCard, count);
                }
                else
                {
                    tempCardList.Add(card);
                    tempCardDict.Add(card, 1);
                }
            }
            List<string> tempDeckList = new List<string>();
            foreach (var card in tempCardDict)
            {
                tempDeckList.Add(card.Value + " " + card.Key);
            }
            var tempStringListNames = String.Join("|", tempDeckList.ToArray());

            string[] cardNames = tempCardList.ToArray();

            int addedCount = 1;
            var entity = new Deck()
            {
                OwnerId = _userId,
                CardListString = tempStringListNames,
                Name = model.Name
            };

            using (var ctx = new ApplicationDbContext())
            {
                foreach (var card in cardNames)
                {
                    if (card != "")
                    {
                        var apiId = CheckIfCardApiExists(card);
                        if (apiId == 0)
                        {
                            newCardsForApi.Add(card);
                            continue;
                        }
                        for (int i = 0; i < tempCardDict[card]; i++)
                        {
                            var cardObject = new Card()
                            {
                                OwnerId = _userId,
                                Name = card,
                                DeckId = entity.DeckId,
                                CardApiId = apiId
                            };
                            ctx.Cards.Add(cardObject);
                            deckList.Add(cardObject.CardId);
                            addedCount += 1;
                        }
                    }
                }

                string newCardsForApiString;
                if (newCardsForApi.Count < 10)
                {
                    newCardsForApiString = String.Join("|", newCardsForApi.ToArray());
                    FindCardListWithApi(newCardsForApiString);
                }
                else
                {
                    var reducedCardList = new List<string>();
                    foreach (var card in newCardsForApi)
                    {
                        reducedCardList.Add(card);
                        if (reducedCardList.Count == 10)
                        {
                            newCardsForApiString = String.Join("|", reducedCardList.ToArray());
                            FindCardListWithApi(newCardsForApiString);
                            reducedCardList.Clear();
                        }
                    }
                    newCardsForApiString = String.Join("|", reducedCardList.ToArray());
                    FindCardListWithApi(newCardsForApiString);
                }



                if (newCardsForApi.Count > 0)
                {
                    foreach (var card in newCardsForApi)
                    {
                        var apiId = CheckIfCardApiExists(card);
                        if (apiId == 0)
                        {
                            continue;
                        }
                        for (int i = 0; i < tempCardDict[card]; i++)
                        {
                            var cardObject = new Card()
                            {
                                OwnerId = _userId,
                                Name = card,
                                DeckId = entity.DeckId,
                                CardApiId = apiId
                            };
                            ctx.Cards.Add(cardObject);
                            deckList.Add(cardObject.CardId);
                            addedCount += 1;
                        }
                    }
                }

                entity.ListOfCards = JsonConvert.SerializeObject(deckList);
                ctx.Decks.Add(entity);
                if (addedCount == ctx.SaveChanges())
                {
                    return entity.DeckId;
                }
                return -1;
            }
        }

        public IEnumerable<CollectionItem> GetDeck(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                    .Cards
                    .Where(e => e.OwnerId == _userId && e.DeckId == id)
                    .Select(
                        e =>
                            new CollectionItem
                            {
                                CardId = e.CardId,
                                Name = e.Name,
                                Printing = e.Printing,
                                MultiverseId = e.MultiverseId,
                                CardCondition = e.CardCondition,
                                IsFoil = e.IsFoil,
                                InUse = e.InUse,
                                ForTrade = e.ForTrade,
                                CardApiId = e.CardApiId,
                                DeckId = e.DeckId
                            }
                    );
                return query.ToArray();
            }
        }

        public DeckItem GetDeckItem(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx
                    .Decks
                    .Single(e => e.OwnerId == _userId && e.DeckId == id);
                if (entity != null)
                {
                    return
                        new DeckItem
                        {
                            DeckId = entity.DeckId,
                            Name = entity.Name,
                            OwnerId = entity.OwnerId,
                            CardListString = entity.CardListString
                        };
                }
                else return
                       new DeckItem
                       {
                           DeckId = -1,
                           Name = "No Deck",
                           OwnerId = _userId,
                           CardListString = ""
                       };
            }

        }

        public Dictionary<int, CardApiItem> GetDeckApiDictionary(IEnumerable<CollectionItem> cardArray)
        {
            Dictionary<int, CardApiItem> apiDict = new Dictionary<int, CardApiItem>();

            using (var ctx = new ApplicationDbContext())
            {
                foreach (var card in cardArray)
                {
                    if (apiDict.ContainsKey(card.CardApiId.Value) == false)
                    {
                        var entity = ctx
                              .CardApis
                              .Single(e => e.CardApiId == card.CardApiId.Value);
                        var carpApiTemp = new CardApiItem
                        {
                            CardApiId = entity.CardApiId,
                            Name = entity.Name,
                            ManaCost = entity.ManaCost,
                            Colors = entity.Colors,
                            Type = entity.Type,
                            Subtypes = entity.Subtypes,
                            Text = entity.Text,
                            Printings = entity.Printings,
                            MultiSetDict = entity.MultiSetDict,
                            SetNameDict = entity.SetNameDict
                        };
                        apiDict.Add(card.CardApiId.Value, carpApiTemp);
                    }
                }
                return apiDict;
            }
        }

        public Dictionary<string, int> GetAllDecksDictionary()
        {
            Dictionary<string, int> deckDict = new Dictionary<string, int>();

            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx
                    .Decks
                    .Where(e => e.OwnerId == _userId)
                    .OrderBy(e => e.Name);
                deckDict.Add("-No Deck-", -1);
                foreach (var deck in entity)
                {
                    deckDict.Add(deck.Name, deck.DeckId);
                }
                return deckDict;
            }
        }

        public bool DeleteDeck(int deckId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                //remove assinged deck from Cards
                int changeCount = 1;
                var entity =
                    ctx
                        .Cards
                        .Where(e => e.OwnerId == _userId && e.DeckId == deckId);

                foreach (var card in entity)
                {
                    card.DeckId = null;
                    changeCount += 1;
                }

                //delete deck
                var deckEntity = ctx
                    .Decks
                    .Single(e => e.DeckId == deckId && e.OwnerId == _userId);

                ctx.Decks.Remove(deckEntity);

                return ctx.SaveChanges() == changeCount;
            }
        }

        public bool DeleteDeckComplete(int deckId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                //Delete cards in the deck
                int changeCount = 1;
                var entity =
                    ctx
                        .Cards
                        .Where(e => e.OwnerId == _userId && e.DeckId == deckId);

                foreach (var card in entity)
                {
                    ctx.Cards.Remove(card);
                    changeCount += 1;
                }

                //delete deck
                var deckEntity = ctx
                    .Decks
                    .Single(e => e.DeckId == deckId && e.OwnerId == _userId);

                ctx.Decks.Remove(deckEntity);

                return ctx.SaveChanges() == changeCount;
            }
        }
    }
}

