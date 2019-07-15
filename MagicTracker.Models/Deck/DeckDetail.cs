using MagicTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models.Deck
{
    public class DeckDetail
    {
        public int DeckId { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public string CardListString { get; set; }
        public List<Card> ListOfCards { get; set; }
    }
}
