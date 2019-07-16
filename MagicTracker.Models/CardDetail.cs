using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models
{
    public enum Condition
    {
        NearMint = 1,
        LightlyPlayed,
        ModeratelyPlayed,
        HeavilyPlayed,
        Damaged,
        Default
    }
    public class CardDetail
    {
        public int CardId { get; set; }
        public string Name { get; set; }
        public string Printing { get; set; }
        public int MultiverseId { get; set; }
        [Display(Name = "Card Condition")]
        public Condition CardCondition { get; set; }
        [Display(Name = "Foil")]
        public bool IsFoil { get; set; }
        [Display(Name = "In Use")]
        public bool InUse { get; set; }
        [Display(Name = "For Trade")]
        public bool ForTrade { get; set; }
        [Display(Name = "Current Holder")]
        public int Holder { get; set; }
        public int? DeckId { get; set; }
    }
}
