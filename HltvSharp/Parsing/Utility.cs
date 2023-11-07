using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HltvSharp.Parsing
{
    public static partial class HltvParser
    {
        public static async Task<string> FetchPage(string url)
        {
			using var browserFetcher = new BrowserFetcher();
			await browserFetcher.DownloadAsync();
			await using var browser = await Puppeteer.LaunchAsync(
				new LaunchOptions { Headless = true });
			await using var page = await browser.NewPageAsync();
			await page.GoToAsync("https://www.hltv.org/" + url);
            Thread.Sleep(3000);
            await page.ScreenshotAsync("sc.png");

            return await page.GetContentAsync();
		}

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        public static long GetCurrentUnixTimestampSeconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }
    }
}
