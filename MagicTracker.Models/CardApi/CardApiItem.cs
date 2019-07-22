using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models.CardApi
{
    public class CardApiItem
    {
        public int CardApiId { get; set; }
        public string Name { get; set; }

        [Display(Name = "Mana Cost")]
        public string ManaCost { get; set; }
        public string Colors { get; set; }
        public string Type { get; set; }
        public string Subtypes { get; set; }
        public string Text { get; set; }
        public string Printings { get; set; }
        public string MultiSetDict { get; set; }
        public string SetNameDict { get; set; }
    }
}
