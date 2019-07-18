using MagicTracker.Models.CardApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Models
{
    public class ApiDeckView
    {
        public CollectionItem[] Deck { get; set; }
        public Dictionary<int, CardApiItem> ApiDict { get; set; }
    }
}
