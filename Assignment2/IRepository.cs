using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Assignment2.Models;

namespace Assignment2 {
	public interface IRepository {
		Task<Player> GetPlayer (Guid id);
		Task<Player[]> GetAllPlayers ();
		Task<Player> CreatePlayer (Player player);
		Task<Player> ModifyPlayer (Guid id, ModifiedPlayer player);
		Task<Player> DeletePlayer (Guid id);
		
		Task<Item> GetItem (Guid playerId, Guid itemId);
		Task<Item[]> GetAllItems (Guid playerId);
		Task<Item> CreateItem (Guid playerId, Item item);
		Task<Item> ModifyItem (Guid playerId, Guid itemId, ModifiedItem item);
		Task<Item> DeleteItem (Guid playerId, Guid itemId);
	}
}
