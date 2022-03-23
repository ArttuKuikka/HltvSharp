// See https://aka.ms/new-console-template for more information
var text = "ence";//Console.ReadLine();

var Search = new HltvSharp.Search();
var res = await Search.Teams(text);

var t = await HltvSharp.Parsing.HltvParser.GetTeam(res[0].Id);
Console.WriteLine(t.Name);