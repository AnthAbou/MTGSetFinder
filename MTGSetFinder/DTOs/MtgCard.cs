using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGSetFinder.DTOs
{
    internal class MtgCard
    {
        public string Name { get; set; }
        public List<MtgCardSetInfo> SetInfo { get; set; } = new List<MtgCardSetInfo>();
        public Guid oracle_id { get; set; }
        public double LowestPrice => SetInfo.Min(x => x.LowestPrice);
        public double HighestPrice => SetInfo.Max(x => x.HighestPrice);
    }
}
