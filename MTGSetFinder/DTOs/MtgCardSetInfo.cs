﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGSetFinder.DTOs
{
    internal class MtgCardSetInfo
    {
        public string SetName { get; set; }
        public string SetNumber { get; set; }
        public string Rarity { get; set; }
        public Guid oracle_id { get; set; }
        
    }
}
