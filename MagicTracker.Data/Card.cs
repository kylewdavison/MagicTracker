using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        Damaged,
        Default
    }
    public class Card
    {
        [Key]
        public Guid CardId { get; set; }
        [Required]
        public Guid OwnerId { get; set; }
        //[ForeignKey("MultiverseId")]
        public int MultiverseId { get; set; }
        public string Name { get; set; }
        public Condition CardCondition { get; set; }
        public bool IsFoil { get; set; }
        public bool InUse { get; set; }
        public bool ForTrade { get; set; }
        public int Holder { get; set; }
    }
}
