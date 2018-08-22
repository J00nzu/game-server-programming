using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Assignment1 {
	public class RealTimeCityBikeDataFetcher : ICityBikeDataFetcher {

		class BikeRentalStationsList {
			public List<StationData> stations;
		}

		class StationData {
			public int id;
			public string name;
			public double x;
			public double y;
			public int bikesAvailable;
			public int spacesAvailable;
			public bool allowDropoff;
			public string state;

			public override string ToString() {
				return base.ToString() + "\n" +
				       "id : " + id + "\n" +
				       "name : " + name + "\n" +
				       "bikesAvailable : " + bikesAvailable + "\n";
			}
		}

		public async Task<int> GetBikeCountInStation(string stationName) {

			if (stationName.Any(char.IsDigit)) {
				throw new ArgumentException("Given station name contains numbers");
			}

			HttpClient client = new HttpClient();

			var gettask =
				await client.GetAsync(new Uri("http://api.digitransit.fi/routing/v1/routers/hsl/bike_rental"));
			
			foreach (
				var bikeData in (
				JsonConvert.DeserializeObject(await gettask.Content.ReadAsStringAsync(),
				typeof(BikeRentalStationsList)) as BikeRentalStationsList).stations
				) 
			{

				if (bikeData.name.Contains(stationName)) {
					return bikeData.bikesAvailable;
				}
			}


			throw new NotFoundException("Given station name was not found");
		}

	}

}
