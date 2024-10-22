using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTGSetFinder.DTOs;
using Refit;

namespace MTGSetFinder.Interfaces
{
    internal interface IScryfallApi
    {
        [Headers("User-Agent: MTGSetFinder/0.1")]
        [Get("/cards/named?exact={name}")]
        Task<ScryfallCard> GetCardByName(string name);
        [Headers("User-Agent: MTGSetFinder/0.1")]
        [Get("/cards/search?order=released&q=oracleid%3A{oracle_id}&unique=prints")]
        Task<ScryfallListResponse> GetSetsForCardByOracleID(Guid oracle_id);
    }
}
