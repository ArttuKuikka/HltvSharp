![logo](https://raw.githubusercontent.com/ArttuKuikka/HltvSharp/master/github_hltvsharp_logo.png)


# HltvSharp

# Nuget Package https://www.nuget.org/packages/HltvSharp/

Hltv is a C# api for getting Hltv info straight from hltv.org. HltvSharp uses mostly Html Agility Pack for parsing the site info and as of 14.3.2022 it should work correctly but i you find something wrong with it open a new issue or message me on discord Arttu#6180.


--Sample Code--

```
var SearchQuery = "ence";

var Search = new HltvSharp.Search();
var res = await Search.Teams(SearchQuery);

var team0 = HltvSharp.Parsing.HltvParser.GetTeam(res[0].Id);

Console.WriteLine(team0.Result.Name);

Console.WriteLine(team0.Result.Id);

Console.WriteLine(team0.Result.WorldRank);

Console.WriteLine(team0.Result.Players[0].Id);

var player0 = HltvSharp.Parsing.HltvParser.GetPlayer(team0.Result.Players[0].Id);
Console.WriteLine(player0.Result.Name);
```
The sample code should return the following output

```
ENCE
4869
15
16080
Pawel 'dycha' Dycha
```

