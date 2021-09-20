using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace QuestAPIPerformanceTesting
{
	class Program
	{
		private static bool active;
		private static double rate = 20;
		private static DateTime nextFetchTime;

		private static void Main(string[] args)
		{
			active = true;
			nextFetchTime = DateTime.Now + TimeSpan.FromSeconds(1f / rate);
			Thread fetchThread = new Thread(FetchThread);
			fetchThread.Start();
		}

		private static void FetchThread()
		{
			while (active)
			{
				DateTime now = DateTime.Now;
				if (now > nextFetchTime)
				{
					try
					{
						WebRequest request = WebRequest.Create($"http://192.168.1.147:6721/session");
						Task.Run(async () =>
						{
							WebResponse response = await request.GetResponseAsync();
							Console.WriteLine(response.ContentLength);
						});
					}
					catch (Exception)
					{
						// ignored
					}

					nextFetchTime += TimeSpan.FromSeconds(1f / rate);
					if (now > nextFetchTime)
					{
						Console.WriteLine($"Missed cycle. {(now - nextFetchTime).Milliseconds}");
						nextFetchTime = now + TimeSpan.FromSeconds(1f / rate);
					}
				}
			}
		}
	}
}