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
                                CardName = e.Name
                            }
                    );
                return query.ToArray();
            }
        }
    }
}
