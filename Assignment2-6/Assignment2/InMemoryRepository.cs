using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assignment2.Models;
using Assignment5;
using Microsoft.CodeAnalysis.CSharp;

namespace Assignment2 {
	class InMemoryRepository : IRepository {
		private const int timeOut = 10;

		private ReaderWriterLock Lock;
		private Dictionary<Guid, Player> players;

		public InMemoryRepository () {
			Lock = new ReaderWriterLock();
			players = new Dictionary<Guid, Player>();
		}

		public async Task<Player> GetPlayer(Guid id) {
			Lock.AcquireReaderLock(timeOut);

			try {
				return players[id];
			}
			finally {
				Lock.ReleaseReaderLock();
			}

		}

		public async Task<Player> GetPlayer<T> (Filter<Player, T> filter) {
			Player[] players = await GetAllPlayers();
			foreach (Player player in players) {
				if (filter.Satisfy(player)) {
					return player;
				}
			}
			return null;
		}


		public async Task<Player[]> GetAllPlayers() {
			Lock.AcquireReaderLock(timeOut);
			Player[] ret = players.Values.ToArray();
			Lock.ReleaseReaderLock();

			return ret;
		}

		public async Task<Player[]> GetAllPlayers<T> (Filter<Player, T> filter) {
			//no need for lock since it uses GetAllPlayers internally and doesn't access the main dictionary directly
			List<Player> filtered = new List<Player>();
			Player[] allplayers = await GetAllPlayers();

			foreach (Player player in allplayers) {
				if (filter.Satisfy(player)) {
					filtered.Add(player);
				}
			}

			return filtered.ToArray();
		}

		public async Task<Player[]> GetPlayersSortedByScore (int num, SortOrder order) {
			Lock.AcquireReaderLock(timeOut);
			Player[] toReturn;
			try {
				switch (order) {
					case SortOrder.Ascending:
						toReturn = players.Values.OrderBy(x => x.Score).Take(num).ToArray();
						break;
					case SortOrder.Descending:
						toReturn = players.Values.OrderByDescending(x => x.Score).Take(num).ToArray();
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(order), order, null);
				}
			}
			finally {
				Lock.ReleaseReaderLock();
			}
			return toReturn;
		}

		public async Task<Player> CreatePlayer(Player player) {
			Lock.AcquireWriterLock(timeOut);
			players.Add(player.Id, player);
			Lock.ReleaseWriterLock();
			return player;
		}

		public async Task<Player> ModifyPlayer(Guid id, ModifiedPlayer player) {
			Lock.AcquireWriterLock(timeOut);

			try {
				if (player.Score != null) {
					players[id].Score = player.Score.Value;
				}
				if (player.Level != null) {
					players[id].Level = player.Level.Value;
				}
				return players[id];
			}
			finally {
				Lock.ReleaseWriterLock();
			}
		}

		public async Task<Player> DeletePlayer(Guid id) {
			Lock.AcquireWriterLock(timeOut);

			try {
				Player play = players[id];
				players.Remove(id);
				return play;
			} finally {
				Lock.ReleaseWriterLock();
			}
		}

		public async Task IncrementScorePlayer(Guid id, PlayerScoreIncrement scoreIncrement) {
			Lock.AcquireWriterLock(timeOut);
			try {
				Player play = players[id];
				play.Score += scoreIncrement.ScoreIncrement;
			} finally {
				Lock.ReleaseWriterLock();
			}
		}

		public async Task NameChangePlayer(Guid id, PlayerNameUpdate nameChange) {
			Lock.AcquireWriterLock(timeOut);
			try {
				Player play = players[id];
				play.Name = nameChange.Name;
			} finally {
				Lock.ReleaseWriterLock();
			}
		}

		public async Task<int> GetAverageScoreForPlayersBetweenDates(DateTime start, DateTime end) {
			int totalScore = 0;
			int numPlayers = 0;

			foreach (Player player in players.Values) {
				if (player.CreationTime > start && player.CreationTime < end) {
					numPlayers++;
					totalScore += player.Score;
				}
			}

			if (numPlayers == 0) // to avoid division by zero
				return 0;
			return totalScore / numPlayers;
		}


		public async Task<Item> GetItem(Guid playerId, Guid itemId) {
			Lock.AcquireReaderLock(timeOut);

			try {
				Item item = GetItemByID(playerId, itemId);

				//Not sure if beneficial. Might just create problems.
				/*if (item.ownerId == playerId) { 
					return item;
				} else {
					return null;
				}*/

				return item;
			}  finally {
				Lock.ReleaseReaderLock();
			}
		}

		public async Task<Item[]> GetAllItems(Guid playerId) {
			Lock.AcquireReaderLock(timeOut);
			Item[] ret = players[playerId].Items.ToArray();
			Lock.ReleaseReaderLock();

			return ret;
		}

		public async Task<Item> CreateItem(Guid playerId, Item item) {
			Lock.AcquireWriterLock(timeOut);
			players[playerId].Items.Add(item);
			Lock.ReleaseWriterLock();
			return item;
		}

		public async Task<Item> ModifyItem(Guid playerId, Guid itemId, ModifiedItem item) {
			Lock.AcquireWriterLock(timeOut);

			try {
				Item i = GetItemByID(playerId, itemId);
				i.Level = item.Level;
				return i;
			} finally {
				Lock.ReleaseWriterLock();
			}
		}

		public async Task<Item> DeleteItem(Guid playerId, Guid itemId) {
			Lock.AcquireWriterLock(timeOut);

			try {
				Player player;
				Item item = GetItemByID(playerId, itemId, out player);
				player.Items.Remove(item);
				return item;
			}finally {
				Lock.ReleaseWriterLock();
			}
		}

		private Item GetItemByID (Guid playerId, Guid itemId) {
			Player player;
			return GetItemByID(playerId, itemId, out player);
		}

		private Item GetItemByID (Guid playerId, Guid itemId, out Player player) {
			player = players[playerId];

			foreach (Item playerItem in player.Items) {
				if (playerItem.Id == itemId) {
					return playerItem;
				}
			}

			return null;
		}
	}
}
