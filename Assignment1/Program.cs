using System;

namespace Assignment1 {
	class Program {
		static void Main (string[] args) {

			string mode;
			string stationName;

			try {
				mode = args[0];
				stationName = args[1];
			}
			catch (Exception ex) {
				Console.WriteLine("Not enough arguments! use 'offline|realtime station_name'");
				return;
			}

			ICityBikeDataFetcher fetcher;

			if (mode == "offline") {
				fetcher = new OfflineCityBikeDataFetcher();
			} else if (mode == "realtime") {
				fetcher = new RealTimeCityBikeDataFetcher();
			} else {
				Console.WriteLine("Mode argument was not proper. Use 'offline|realtime'");
				return;
			}

			try {
				var task = fetcher.GetBikeCountInStation(stationName);
				task.Wait();
				Console.WriteLine(task.Result);
			} catch (AggregateException ae) {
				foreach (var ex in ae.InnerExceptions) {
					if (ex is NotFoundException) {
						Console.WriteLine("Not found: " + ex.Message);
					} else if (ex is ArgumentException) {
						Console.WriteLine("Invalid Arguemnt: " + ex.Message);
					} else {
						Console.WriteLine("something went wrong.");
					}
				}
			}
#if DEBUG
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
#endif
		}
	}
}
