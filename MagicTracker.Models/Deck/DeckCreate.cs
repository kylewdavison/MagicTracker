using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models.Deck
{
    public class DeckCreate
    {
        [Display(Name = "Deck List")]
        public string CardListString { get; set; }
        public string Name { get; set; }
    }
}
