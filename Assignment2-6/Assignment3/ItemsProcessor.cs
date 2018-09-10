using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Models;

namespace Assignment2.Processors {

	public class ItemsProcessor {

		private readonly IRepository _repository;

		public ItemsProcessor (IRepository repository) {
			this._repository = repository;
		}

		public Task<Item> Get (Guid playerId, Guid itemId) {
			return _repository.GetItem(playerId, itemId);
		}

		public Task<Item[]> GetAll (Guid playerId) {
			return _repository.GetAllItems(playerId);
		}

		public Task<Item> Create (Guid playerId, NewItem item) {
			Player player = _repository.GetPlayer(playerId).Result;
			if (item.Type == ItemType.Sword && player.Level < 3) {
				throw new InvalidPlayerLevelException("An item of type 'Sword' is not allowed for players below level 3");
			}
			Item nuItem = new Item {
				Id = Guid.NewGuid(),
				Level = item.Level,
				Type = item.Type,
				CreationDate = item.CreationDate,
				ownerId = playerId,
				Name = item.Name
			};

			return _repository.CreateItem(playerId, nuItem);

		}
		public Task<Item> Modify (Guid playerId, Guid itemId, ModifiedItem item) {
			return _repository.ModifyItem(playerId, itemId, item);
		}
		public Task<Item> Delete (Guid playerId, Guid itemId) {
			return _repository.DeleteItem(playerId, itemId);
		}
	}
}
