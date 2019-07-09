using MagicTracker.Data;
using MagicTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
