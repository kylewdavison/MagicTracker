using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Data
{
    public enum Condition
    {
        NearMint = 1,
        LightlyPlayed,
        ModeratelyPlayed,
        HeavilyPlayed,
        Damaged
    }
    public class Card
    {
        [Key]
        public int CardId { get; set; }
        [Required]
        public Guid OwnerId { get; set; }
        public int MultiverseId { get; set; }
        public string Name { get; set; }
        public string Printing { get; set; }
        public Condition CardCondition { get; set; }
        public bool IsFoil { get; set; }
        public bool InUse { get; set; }
        public bool ForTrade { get; set; }
        [ForeignKey(nameof(Deck))]
        public int? DeckId { get; set; }
        public virtual Deck Deck { get; set; }
        [ForeignKey(nameof(CardApi))]
        public int? CardApiId { get; set; }
        public virtual CardApi CardApi { get; set; }
    }
}
