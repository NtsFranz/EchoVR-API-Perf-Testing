using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace QuestAPIPerformanceTesting
{
	class Program
	{
		private const string QUEST_IP = "192.168.1.147";
		private const double RATE = 60;

		private static DateTime nextFetchTime;

		private static void Main(string[] args)
		{
			nextFetchTime = DateTime.Now + TimeSpan.FromSeconds(1f / RATE);
			Thread fetchThread = new Thread(FetchThread);
			fetchThread.Start();
		}

		private static void FetchThread()
		{
			while (true)
			{
				DateTime now = DateTime.Now;
				if (now > nextFetchTime)
				{
					try
					{
						WebRequest request = WebRequest.Create($"http://{QUEST_IP}:6721/session");
						Task.Run(async () =>
						{
							WebResponse response = await request.GetResponseAsync();
							Console.WriteLine(response.ContentLength);
						});
					}
					catch (Exception)
					{
						// Not in game
					}

					nextFetchTime += TimeSpan.FromSeconds(1f / RATE);
					if (now > nextFetchTime)
					{
						Console.WriteLine($"Missed cycle. {(now - nextFetchTime).Milliseconds}");
						nextFetchTime = now + TimeSpan.FromSeconds(1f / RATE);
					}
				}
			}
		}
	}
}