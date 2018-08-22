using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1 {

	public class OfflineCityBikeDataFetcher  : ICityBikeDataFetcher {
		private string path = "bikedata.txt";

		public async Task<int> GetBikeCountInStation(string stationName) {

			if (stationName.Any(char.IsDigit)) {
				throw new ArgumentException("Given station name contains numbers");
			}

			string[] lines = await System.IO.File.ReadAllLinesAsync(path);

			foreach (string line in lines) {
				string[] items = line.Split(":");
				if (items[0].Contains(stationName)) {
					return int.Parse(items[1].Trim());
				}
			}

			throw new NotFoundException("Given station name was not found");
		}
	}
}
