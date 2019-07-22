using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models.CardApi
{
    public class ApiCardView
    {
        public CollectionItem Card { get; set; }
        public CardApiItem Api { get; set; }
        public Dictionary<string, int> DeckDict { get; set; }
        [Display(Name = "Deck Name")]
        public string DeckName { get; set; }
    }
}
