using MTGSetFinder.DTOs;
using MTGSetFinder.Interfaces;
using Refit;
using System.Text;

List<string> cardNames = new List<string>();
var scryfallApi = RestService.For<IScryfallApi>("https://api.scryfall.com");
var allCards = new List<MtgCard>();
var allSets = new List<MtgCardSetInfo>();
var readPath = @"%userprofile%\mtgcards.txt";
var writePath = @"%userprofile%\cardswithsets.txt";

Console.WriteLine("getting cards from file...");
using (var inputReader = new StreamReader(Environment.ExpandEnvironmentVariables(readPath)))
{
    while (!inputReader.EndOfStream)
    {
        var line = inputReader.ReadLine();
        //skip cards that have way too many fucking printings
        if (line.ToLowerInvariant() == "sol ring" || line.ToLowerInvariant() == "plains" || line.ToLowerInvariant() == "island" || line.ToLowerInvariant() == "swamp" || line.ToLowerInvariant() == "mountain" || line.ToLowerInvariant() == "forest" || line.ToLowerInvariant() == "wastes" || line.ToLowerInvariant() == "command tower")
        {
            var cardId = Guid.NewGuid();
            allCards.Add(
                new MtgCard()
                {
                    Name = line,
                    oracle_id = cardId,
                    SetInfo = new List<MtgCardSetInfo>() {
                        new MtgCardSetInfo() {
                            SetName = "excluded",
                            oracle_id = cardId
                        }
                    }
                });
            allSets.Add(new MtgCardSetInfo()
            {
                SetName = "excluded",
                oracle_id = cardId
            });
        }
        else
            cardNames.Add(line);
    }
}
//get card and set info
Console.WriteLine("looking up cards and sets...");
foreach (var cardName in cardNames)
{
    Console.WriteLine($"Looking up: {cardName}");
    var baseCard = await scryfallApi.GetCardByName(cardName);
    Console.WriteLine($"Oracle id: {baseCard.oracle_id}");
    var scryFallCardsByOracleID = await scryfallApi.GetSetsForCardByOracleID(baseCard.oracle_id);

    var newCard = new MtgCard();
    newCard.Name = baseCard.name;
    newCard.oracle_id = baseCard.oracle_id;
    var addedSetNamesForThisCard = new List<string>();
    
    foreach (var scryfallCard in scryFallCardsByOracleID.data)
    {
        //exclude secret lair drops and the list
        if (scryfallCard.set.ToLowerInvariant() == "sld" || scryfallCard.set.ToLowerInvariant() == "prm" || scryfallCard.set.ToLowerInvariant() == "plst" || scryfallCard.set.ToLowerInvariant().Contains("wc") || scryfallCard.set.Length > 3 || scryfallCard.set.ToLowerInvariant() == "ptc")
            continue;

        addedSetNamesForThisCard.Add(scryfallCard.set);

        var setInfo = new MtgCardSetInfo()
        {
            SetName = scryfallCard.set,
            oracle_id = scryfallCard.oracle_id,
            Rarity = scryfallCard.rarity,
            //SetNumber = Encoding.UTF8.GetString(Encoding.Default.GetBytes(scryfallCard.collector_number)),
            SetNumber = scryfallCard.collector_number,
            CardPrice = new ScryfallPrice()
            {
                usd = scryfallCard.prices.usd,
                usd_foil = scryfallCard.prices.usd_foil,
                usd_etched = scryfallCard.prices.usd_etched
            }
        };
        newCard.SetInfo.Add(setInfo);
        allSets.Add(setInfo);
    }
    newCard.SetInfo = newCard.SetInfo.OrderBy(o => o.LowestPrice).ThenBy(o => o.SetName).ToList();
    allCards.Add(newCard);
}

Console.WriteLine("sorting");
//determine which sets have the most cards
var distinctSets = allCards.Select(o => o.SetInfo.Select(o => o.SetName).Distinct()).ToList();

var groupedSetInfo = allSets.GroupBy(o => o.SetName).ToList().OrderByDescending(g => g.Count()).ToList();
var setNameSortOrder = groupedSetInfo.Select(x => x.Key).ToList();

StringBuilder sb = new StringBuilder();

//remove card id from every set it exists in after adding line
var finalizedList = new List<FinalizedCard>();
var alreadyAddedCards = new List<Guid>();
sb.AppendLine("Card Name\tPull List\tLowest Price\tHigest Price\tSet1\tSet2\tSet3\tSet4\tSet5");
foreach (var set in groupedSetInfo)
{
    foreach (var info in set)
    {
        if (!alreadyAddedCards.Contains(info.oracle_id)) 
        {
            var card = allCards.Where(s => s.oracle_id == info.oracle_id).FirstOrDefault();
            if (card is null)
                continue;

            var cardWithAllSets = $"{card.Name} - ";
            sb.Append($"{card.Name}\t");
            sb.Append("{placeholder}\t");
            sb.Append($"{card.LowestPrice}\t{card.HighestPrice}\t");

            card.SetInfo = card.SetInfo.OrderBy(o => o.LowestPrice).ThenBy(s => setNameSortOrder.IndexOf(s.SetName)).ToList();
            //card.SetInfo = card.SetInfo.OrderBy(s => setNameSortOrder.IndexOf(s.SetName)).ThenBy(o => o.LowestPrice).ToList();
            int setCount = 0;
            foreach (var setinfo in card.SetInfo)
            {
                if (setCount < 5)
                {
                    cardWithAllSets += ($"({setinfo.SetName}-{setinfo.SetNumber}) ");
                    sb.Append($"{setinfo.SetName}-{setinfo.SetNumber} ({setinfo.CardPrice.usd ?? "-"} | {setinfo.CardPrice.usd_foil ?? "-"} | {setinfo.CardPrice.usd_etched ?? "-"}) \t");

                    setCount++;
                }
                else
                    continue;
            }
            sb.Replace("{placeholder}", cardWithAllSets);
            sb.AppendLine();
            alreadyAddedCards.Add(info.oracle_id);
        }
    }
}

//write to file
Console.WriteLine("writing");
using (StreamWriter outputFile = new StreamWriter(Environment.ExpandEnvironmentVariables(writePath)))
{
    outputFile.WriteLine(sb.ToString());
}
Console.WriteLine("done");
