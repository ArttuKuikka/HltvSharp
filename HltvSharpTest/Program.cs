// See https://aka.ms/new-console-template for more information
using System.Net;

var text = "ence";//Console.ReadLine();



var Search = new HltvSharp.Search();
var res = await Search.Teams(text);
var res2 = await Search.Teams("havu");

var t = await HltvSharp.Parsing.HltvParser.GetTeam(res[0].Id);
Console.WriteLine(t.Name);

var t2 = await HltvSharp.Parsing.HltvParser.GetTeam(res2[0].Id);
Console.WriteLine(t2.Name);
////foreach(var match in t.RecentMatches)
////{
////    Console.WriteLine(match.date);
////}

//var m = await HltvSharp.Parsing.HltvParser.GetMatch(2354555);
//Console.WriteLine(m.Id);

//var proo = await HltvSharp.Parsing.HltvParser.GetRankings();

