using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2 {
	public class PlayersProcessor {
		private readonly IRepository _repository;

		public PlayersProcessor (IRepository repository) {
			this._repository = repository;
		}

		public Task<Player> Get (Guid id) {
			return _repository.GetPlayer(id);
		}

		public Task<Player[]> GetAll () {
			return _repository.GetAllPlayers();
		}

		public Task<Player> Create (NewPlayer player) {
			Player nuPlayer = new Player();
			nuPlayer.Id = Guid.NewGuid();
			nuPlayer.Score = 0;
			nuPlayer.Level = 1;
			nuPlayer.Name = player.Name;
			nuPlayer.IsBanned = false;
			nuPlayer.CreationTime = DateTime.Now;

			return _repository.CreatePlayer(nuPlayer);
		}
		public Task<Player> Modify (Guid id, ModifiedPlayer player) {
			return _repository.ModifyPlayer(id, player);
		}
		public Task<Player> Delete (Guid id) {
			return _repository.DeletePlayer(id);
		}

	}
}
