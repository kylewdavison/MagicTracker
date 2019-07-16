using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Data
{
    public class Deck
    {
        [Key]
        public int DeckId { get; set; }
        public string Name { get; set; }
        [Required]
        public Guid OwnerId { get; set; }
        public string CardListString { get; set; }
        public List<int> ListOfCards { get; set; }

    }
}
