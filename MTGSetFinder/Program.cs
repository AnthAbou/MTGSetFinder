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
var highestSetCount = 0;

Console.WriteLine("getting cards from file...");
using (var inputReader = new StreamReader(Environment.ExpandEnvironmentVariables(readPath)))
{
    while (!inputReader.EndOfStream)
    {
        var line = inputReader.ReadLine();
        //skip cards that have way too many fucking printings
        if (line.ToLowerInvariant() == "sol ring" || line.ToLowerInvariant() == "plains" || line.ToLowerInvariant() == "island" || line.ToLowerInvariant() == "swamp" || line.ToLowerInvariant() == "mountain" || line.ToLowerInvariant() == "forest" || line.ToLowerInvariant() == "wastes" || line.ToLowerInvariant() == "command tower")
            continue;
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
    var sets = await scryfallApi.GetSetsForCardByOracleID(baseCard.oracle_id);

    var newCard = new MtgCard();
    newCard.Name = baseCard.name;
    newCard.oracle_id = baseCard.oracle_id;
    if (sets.data.Count() > highestSetCount)
        highestSetCount = sets.data.Count();
    foreach (var set in sets.data)
    {
        //exclude secret lair drops and the list
        if (set.set.ToLowerInvariant() == "secret lair drop" || set.set.ToLowerInvariant() == "the list")
            continue;
        var setInfo = new MtgCardSetInfo() { Rarity = set.rarity, SetName = set.set, SetNumber = set.collector_number, oracle_id = set.oracle_id };
        newCard.SetInfo.Add(setInfo);
        allSets.Add(setInfo);
    }
    newCard.SetInfo = newCard.SetInfo.OrderBy(o => o.SetName).ThenBy(o => o.SetNumber).ToList();
    allCards.Add(newCard);
}

Console.WriteLine("sorting");
//determine which sets have the most cards
var setDict = new Dictionary<string, int>();
var distinctSets = allCards.Select(o => o.SetInfo.Select(o => o.SetName).Distinct()).ToList();

var groupedSetInfo = allSets.GroupBy(o => o.SetName).ToList().OrderByDescending(g => g.Count()).ToList();
var setNameSortOrder = groupedSetInfo.Select(x => x.Key).ToList();

StringBuilder sb = new StringBuilder();

//remove card id from every set it exists in after adding line
var alreadyAddedCards = new List<Guid>();
Console.WriteLine(highestSetCount);
sb.Append($"Card Name\t");
for (int i=0; i< highestSetCount; i++)
{
    sb.Append("Set Name\tCard Number");
}
sb.AppendLine();
foreach (var set in groupedSetInfo)
{
    foreach (var info in set)
    {
        if (!alreadyAddedCards.Contains(info.oracle_id)) 
        { 
            var card = allCards.Where(s => s.oracle_id == info.oracle_id).FirstOrDefault();
            if (card is null)
                continue;

            sb.Append($"{card.Name}\t");
            card.SetInfo = card.SetInfo.OrderBy(s => setNameSortOrder.IndexOf(s.SetName)).ToList();
            foreach (var setinfo in card.SetInfo)
            {
                sb.Append($"{setinfo.SetName}\t{setinfo.Rarity}-{setinfo.SetNumber}\t");
            }
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
