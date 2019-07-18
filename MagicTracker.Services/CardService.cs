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

namespace MagicTracker.Services
{
    public class CardService
    {
        private readonly Guid _userId;

        public CardService(Guid userId)
        {
            _userId = userId;
        }

        public bool CreateCard(CardCreate model)
        {
            var apiId = CheckIfCardApiExists(model.CardName);
            int tempApiId = -1;
            if (apiId == 0)
            {
                if (FindCardWithApi(model.CardName))
                {
                    tempApiId = CheckIfCardApiExists(model.CardName);
                }
                else return false;
            }
            if (tempApiId != -1)
            {
                apiId = tempApiId;
            }
            var entity = new Card()
            {
                OwnerId = _userId,
                Name = model.CardName,
                CardApiId = apiId
            };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Cards.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public bool CreateDeck(DeckCreate model)
        {
            if (model.CardListString == null) { return false; }
            List<string> tempCardList = new List<string>();
            List<string> newCardsForApi = new List<string>();
            Dictionary<string, int> tempCardDict = new Dictionary<string, int>();
            List<int> deckList = new List<int>();

            string[] tempCardNames = model.CardListString.Split('\n');
            tempCardNames = (from c in tempCardNames
                             select c.Trim()).ToArray();

            foreach (var card in tempCardNames)
            {
                if(card == "") { continue; }
                if (Char.IsDigit(card[0]))
                {
                    int count = int.Parse(Char.ToString(card[0]));
                    var reducedCard = card.Remove(0, 1);
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

            var tempStringListNames = String.Join("|", tempCardList.ToArray());

            string[] cardNames = tempCardList.ToArray();

            int addedCount = 1;
            var entity = new Deck()
            {
                OwnerId = _userId,
                CardListString = tempStringListNames
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

                var newCardsForApiString = String.Join("|", newCardsForApi.ToArray());
                FindCardListWithApi(newCardsForApiString);

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
                entity.ListOfCards = deckList;
                ctx.Decks.Add(entity);
                return addedCount == ctx.SaveChanges();
            }
        }

        public IEnumerable<CollectionItem> GetCollection()
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
                                Name = e.Name,
                                Printing = e.Printing,
                                MultiverseId = e.MultiverseId,
                                CardCondition = (Models.Condition)(int)e.CardCondition,
                                IsFoil = e.IsFoil,
                                InUse = e.InUse,
                                ForTrade = e.ForTrade,
                                DeckId = e.DeckId,
                                CardApiId = e.CardApiId
                            }
                    );
                return query.ToArray();
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
                                CardCondition = (Models.Condition)(int)e.CardCondition,
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
                return
                    new DeckItem
                    {
                        DeckId = entity.DeckId,
                        Name = entity.Name,
                        OwnerId = entity.OwnerId,
                        CardListString = entity.CardListString
                    };
            }

        }

        public Dictionary<int,CardApiItem> GetDeckApiDictionary(IEnumerable<CollectionItem> cardArray)
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
        public IEnumerable<DeckItem> GetAllDecks()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx
                    .Decks
                    .Where(e => e.OwnerId == _userId)
                    .Select(
                        e =>
                            new DeckItem
                            {
                                DeckId = e.DeckId,
                                Name = e.Name,
                                OwnerId = e.OwnerId,
                                CardListString = e.CardListString,
                            }
                    );
                return query.ToArray();
            }
        }

        public CardDetail GetCardById(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {

                var entity =
                    ctx
                        .Cards
                        .Single(e => e.CardId == id && e.OwnerId == _userId);
                return
                    new CardDetail
                    {
                        CardId = entity.CardId,
                        Name = entity.Name,
                        Printing = entity.Printing,
                        CardCondition = (Models.Condition)(int)entity.CardCondition,
                        IsFoil = entity.IsFoil,
                        InUse = entity.InUse,
                        ForTrade = entity.ForTrade,
                        Holder = entity.Holder,
                        MultiverseId = entity.MultiverseId,
                        DeckId = entity.DeckId,
                        CardApiId = entity.CardApiId
                    };
            }
        }

        public bool FindCardWithApi(string card)
        {
            List<mtgCard.Card> listOfResults = new List<mtgCard.Card>();
            mtgService.CardService apiService = new mtgService.CardService();

            CardApi newCard = new CardApi();

            var searchResults = apiService.Where(x => x.Name, card)
                .All();
            if (searchResults.Value.Count != 0)
            {
                if (searchResults.Value[0].Name.ToLower() != card.ToLower()) { return false; }
                newCard.Name = searchResults.Value[0].Name;
                if (searchResults.Value[0].ManaCost == null) { newCard.ManaCost = "{0}"; }
                else { newCard.ManaCost = searchResults.Value[0].ManaCost; }
                newCard.Colors = searchResults.Value[0].Colors;
                newCard.Type = searchResults.Value[0].Type;
                newCard.Subtypes = searchResults.Value[0].SubTypes;
                newCard.Text = searchResults.Value[0].Text;
                newCard.Printings = searchResults.Value[0].Printings;

                Dictionary<int, string> tempMultiSetDict = new Dictionary<int, string>();
                Dictionary<string, string> tempSetNameDict = new Dictionary<string, string>();
                foreach (var result in searchResults.Value)
                {
                    if (result.Name.ToLower() == card.ToLower())
                    {
                        if (result.MultiverseId != null && tempSetNameDict.ContainsKey(result.Set) == false)
                        {
                            tempSetNameDict.Add(result.Set, result.SetName);
                            tempMultiSetDict.Add(result.MultiverseId.Value, result.Set);
                        }
                    }
                }
                newCard.SetNameDict = tempSetNameDict;
                newCard.MultiSetDict = tempMultiSetDict;
                using (var ctx = new ApplicationDbContext())
                {
                    ctx.CardApis.Add(newCard);
                    ctx.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool FindCardListWithApi(string cardListString)
        {
            List<mtgCard.Card> listOfResults = new List<mtgCard.Card>();
            mtgService.CardService apiService = new mtgService.CardService();
            string[] cardNamesArray = cardListString.Split('|');
            List<CardApi> listOfCarApi = new List<CardApi>();

            var searchResults = apiService.Where(x => x.Name, cardListString)
                .All();
            if (searchResults.Value.Count != 0)
            {
                foreach (var card in cardNamesArray)
                {
                    for (int i = 0; i < searchResults.Value.Count; i++)
                    {
                        if (searchResults.Value[i].Name.ToLower() != card.ToLower())
                        {
                            continue;
                        }

                        CardApi newCard = new CardApi();
                        newCard.Name = searchResults.Value[i].Name;
                        if (searchResults.Value[i].ManaCost == null) { newCard.ManaCost = "{0}"; }
                        else { newCard.ManaCost = searchResults.Value[i].ManaCost; }
                        newCard.Colors = searchResults.Value[i].Colors;
                        newCard.Type = searchResults.Value[i].Type;
                        newCard.Subtypes = searchResults.Value[i].SubTypes;
                        newCard.Text = searchResults.Value[i].Text;
                        newCard.Printings = searchResults.Value[i].Printings;

                        Dictionary<int, string> tempMultiSetDict = new Dictionary<int, string>();
                        Dictionary<string, string> tempSetNameDict = new Dictionary<string, string>();
                        foreach (var result in searchResults.Value)
                        {
                            if (result.Name.ToLower() == card.ToLower())
                            {
                                if (result.MultiverseId != null && tempSetNameDict.ContainsKey(result.Set) == false)
                                {
                                    tempSetNameDict.Add(result.Set, result.SetName);
                                    tempMultiSetDict.Add(result.MultiverseId.Value, result.Set);
                                }
                            }
                        }
                        newCard.SetNameDict = tempSetNameDict;
                        newCard.MultiSetDict = tempMultiSetDict;

                        listOfCarApi.Add(newCard);
                        break;
                    }

                }
                using (var ctx = new ApplicationDbContext())
                {
                    foreach (var cardApi in listOfCarApi)
                    {
                        ctx.CardApis.Add(cardApi);
                    }
                    ctx.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public int CheckIfCardApiExists(string name)
        {
            using (var ctx = new ApplicationDbContext())
            {
                try
                {
                    var entity = ctx
                    .CardApis
                    .Single(e => e.Name.ToLower() == name.ToLower());
                    return entity.CardApiId;
                }
                catch (InvalidOperationException)
                {
                    return 0;
                }
                catch (ArgumentNullException)
                {
                    return 0;
                }
            }
        }

        public bool UpdateCard(CardEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx
                    .Cards
                    .Single(e => e.CardId == model.CardId && e.OwnerId == _userId);

                entity.Name = model.Name;
                entity.Printing = model.Printing;
                entity.CardCondition = (Data.Condition)(int)model.CardCondition;
                entity.IsFoil = model.IsFoil;
                entity.InUse = model.InUse;
                entity.ForTrade = model.ForTrade;
                entity.MultiverseId = model.MultiverseId;
                entity.Holder = model.Holder;
                entity.DeckId = model.DeckId;
                entity.CardApiId = model.CardApiId;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool UpdateCards(CardDetailMultiple models)
        {
            using (var ctx = new ApplicationDbContext())
            {
                foreach (var entry in models.CardList)
                {
                    var card = GetCardById(entry.CardId);
                    var entity = ctx
                    .Cards
                    .Single(e => e.CardId == entry.CardId && e.OwnerId == _userId);
                    {
                        entity.CardId = card.CardId;
                        entity.Name = card.Name;
                        entity.Printing = card.Printing;
                        entity.CardCondition = (Data.Condition)(int)card.CardCondition;
                        entity.IsFoil = card.IsFoil;
                        entity.InUse = card.InUse;
                        entity.ForTrade = card.ForTrade;
                        entity.MultiverseId = card.MultiverseId;
                        entity.Holder = card.Holder;
                        entity.DeckId = card.DeckId;
                        entity.CardApiId = card.CardApiId;
                    };
                }

                return ctx.SaveChanges() == 1;
            }

        }

        public bool DeleteCard(int cardId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                        .Cards
                        .Single(e => e.CardId == cardId && e.OwnerId == _userId);

                ctx.Cards.Remove(entity);

                return ctx.SaveChanges() == 1;
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
