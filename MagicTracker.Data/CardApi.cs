﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTracker.Data
{
    public class CardApi
    {
        [Key]
        public int CardApiId { get; set; }
        public string Name { get; set; }
        public string ManaCost { get; set; }
        public string[] Colors { get; set; }
        public string Type { get; set; }
        public string[] Subtypes { get; set; }
        public string Text { get; set; }
        public string[] Printings { get; set; }
        public Dictionary<int, string> MultiSetDict { get; set; }
        public Dictionary<string, string> SetNameDict { get; set; }
    }
}
