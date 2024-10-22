using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGSetFinder.DTOs
{
    internal class ScryfallListResponse
    {
        public List<ScryfallCard> data { get; set; } = new List<ScryfallCard>();
    }
}
