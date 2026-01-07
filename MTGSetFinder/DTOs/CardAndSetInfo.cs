using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGSetFinder.DTOs
{
    internal class CardAndSetInfo
    {
        public CardAndSetInfo()
        {
            
        }
        public CardAndSetInfo(string name, string set, string number, MtgCardSetInfo costs)
        {
            Name = name;
            Set = set;
            Number = number;
            if (costs != null)
            {
                var standard = double.Parse(costs.CardPrice.usd ?? double.MaxValue.ToString());
                var foil = double.Parse(costs.CardPrice.usd_foil ?? double.MaxValue.ToString());
                var etched = double.Parse(costs.CardPrice.usd_etched ?? double.MaxValue.ToString());

                if (Math.Min(standard, foil) == standard && Math.Min(standard, etched) == standard)
                {
                    Cost = standard.ToString();
                    isStandard = true;
                }
                if (Math.Min(foil, standard) == foil && Math.Min(foil, etched) == foil)
                {
                    Cost = foil.ToString();
                    isFoil = true;
                }
                if (Math.Min(etched, standard) == etched && Math.Min(etched, foil) == etched)
                {
                    Cost = etched.ToString();
                    isEtched = true;
                }
            }
        }
        public string Name { get; set; }
        public string Set { get; set; }
        public string Number { get; set; }
        public string Cost { get; set; }
        public bool isStandard { get; set; } = false;
        public bool isFoil { get; set; } = false;
        public bool isEtched { get; set; } = false;
        public string GetCardType() => isStandard ? "" : isFoil ? "Foil" : isEtched ? "Etched" : "";
    }
}
