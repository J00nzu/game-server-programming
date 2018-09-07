using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assignment2.Models;

namespace Assignment2 {
	class InMemoryRepository : IRepository {
		private const int timeOut = 10;

		private ReaderWriterLock Lock;
		private Dictionary<Guid, Player> players;
		private Dictionary<Guid, Item> items;

		public InMemoryRepository () {
			Lock = new ReaderWriterLock();
			players = new Dictionary<Guid, Player>();
			items = new Dictionary<Guid, Item>();
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

		public async Task<Player[]> GetAllPlayers() {
			Lock.AcquireReaderLock(timeOut);
			Player[] ret = players.Values.ToArray();
			Lock.ReleaseReaderLock();

			return ret;
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

		public async Task<Item> GetItem(Guid playerId, Guid itemId) {
			Lock.AcquireReaderLock(timeOut);

			try {
				Item item = items[itemId];

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
			Item[] ret = items.Values.Where(x => x.ownerId == playerId).ToArray();
			Lock.ReleaseReaderLock();

			return ret;
		}

		public async Task<Item> CreateItem(Guid playerId, Item item) {
			Lock.AcquireWriterLock(timeOut);
			items.Add(item.Id, item);
			Lock.ReleaseWriterLock();
			return item;
		}

		public async Task<Item> ModifyItem(Guid playerId, Guid itemId, ModifiedItem item) {
			Lock.AcquireWriterLock(timeOut);

			try {
				Item i = items[itemId];
				i.Level = item.Level;
				return i;
			} finally {
				Lock.ReleaseWriterLock();
			}
		}

		public async Task<Item> DeleteItem(Guid playerId, Guid itemId) {
			Lock.AcquireWriterLock(timeOut);

			try {
				Item item = items[itemId];
				items.Remove(itemId);
				return item;
			}finally {
				Lock.ReleaseWriterLock();
			}
		}
	}
}
