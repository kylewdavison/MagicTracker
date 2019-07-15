using MagicTracker.Data;
using MagicTracker.Models;
using mtgCard = MtgApiManager.Lib.Model;
using mtgService = MtgApiManager.Lib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Card = MagicTracker.Data.Card;

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
            var entity = new Card()
            {
                OwnerId = _userId,
                Name = model.CardName
            };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Cards.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public void FindCardWithApi(Card card)
        {
            List<mtgCard.Card> listOfResults = new List<mtgCard.Card>();
            mtgService.CardService apiService = new mtgService.CardService();

            var searchResults = apiService.Where(x => x.Name, card.Name)
                .All();
            if (searchResults.Value.Count != 0)
            {
                var sets = searchResults.Value[0].Printings;
            }
        }

/*        public IEnumerable<CollectionItem> GetCollectionFullDetails()
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
                return query.ToList();
            }
        }*/

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
                                InUse = e.InUse
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
                        MultiverseId = entity.MultiverseId
                    };
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

    }
}
