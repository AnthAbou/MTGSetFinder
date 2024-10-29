using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGSetFinder.DTOs
{
    internal class FinalizedCard
    {
        public string CardName { get; set; }
        public string CardWithFiveSets { get; set; }
        public string FirstSet { get; set; } 
    }
}
