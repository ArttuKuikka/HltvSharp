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
            var Search = new HltvSharp.Search();
            var id = Search.Players("snappi");

            var task = HltvParser.GetPlayer(id.Result[0].id);
            task.Wait();
            
            Console.WriteLine(task.Result.Name);
            

           
        }
    }
}
