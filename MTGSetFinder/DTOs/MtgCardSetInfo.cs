using System;
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
        public string set { get; set; }

        public ScryfallPrice CardPrice { get; set; } = new ScryfallPrice();

        public double LowestPrice { get => Math.Min(double.Parse(CardPrice.usd ?? double.MaxValue.ToString()), Math.Min(double.Parse(CardPrice.usd_foil ?? double.MaxValue.ToString()), double.Parse(CardPrice.usd_etched ?? double.MaxValue.ToString()))); }

        public double HighestPrice { get => Math.Max(double.Parse(CardPrice.usd ?? double.MinValue.ToString()), Math.Max(double.Parse(CardPrice.usd_foil ?? double.MinValue.ToString()), double.Parse(CardPrice.usd_etched ?? double.MinValue.ToString()))); }
    }
}
