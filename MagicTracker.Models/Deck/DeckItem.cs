using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models.Deck
{
    public class DeckItem
    {
        public int DeckId { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public string CardListString { get; set; }
        public string ListOfCards { get; set; }
    }
}
