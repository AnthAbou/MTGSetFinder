using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MTGSetFinder.DTOs
{
    internal class ScryfallCard
    {
        public string? name { get; set; }
        public Guid oracle_id { get; set; }
        [JsonPropertyName("set_name")]
        public string? set { get; set; }
        public string collector_number { get; set; }
        public string rarity { get; set; }
    }
}
