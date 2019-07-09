using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models
{
    public class CardCreate
    {
        [Required]
        [MinLength(3, ErrorMessage = "Please enter at least 3 characters.")]
        [MaxLength(40, ErrorMessage = "There are too many characters in this field.")]
        public string CardName { get; set; }
        public int MultiverseId { get; set; }
    }
}
