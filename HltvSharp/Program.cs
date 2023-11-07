using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Fizzler.Systems.HtmlAgilityPack;
using System.Linq;
using HltvSharp.Models;
using HltvSharp.Models.Enums;
using HltvSharp.Parsing;

namespace HltvSharp
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
			var text = "ence";//Console.ReadLine();



			var Search = new HltvSharp.Search();
			var res = await Search.Teams(text);


			var t = await HltvSharp.Parsing.HltvParser.GetTeam(res[0].Id);
			Console.WriteLine(t.Name);

			var peli = await HltvSharp.Parsing.HltvParser.GetMatch(2357202);
			var k = peli.Demos.FirstOrDefault().Url;
			Console.WriteLine(k);



		}
    }
}
