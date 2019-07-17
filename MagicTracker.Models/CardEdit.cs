using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models
{
    public class CardEdit
    {
        public int CardId { get; set; }
        public string Name { get; set; }
        public string Printing { get; set; }
        public Condition CardCondition { get; set; }
        public bool IsFoil { get; set; }
        public bool InUse { get; set; }
        public bool ForTrade { get; set; }
        public int MultiverseId { get; set; }
        public int Holder { get; set; }
        public int? DeckId { get; set; }
        public int? CardApiId { get; set; }
    }
}
