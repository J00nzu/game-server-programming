using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Assignment2.Models;
using Assignment5;

namespace Assignment2 {
	public interface IRepository {
		Task<Player> GetPlayer (Guid id);
		Task<Player> GetPlayer <T>(Filter<Player,T> filter);
		Task<Player[]> GetAllPlayers ();
		Task<Player[]> GetAllPlayers<T> (Filter<Player, T> filter);
		Task<Player[]> GetPlayersSortedByScore(int num, SortOrder order);
		Task<Player> CreatePlayer (Player player);
		Task<Player> ModifyPlayer (Guid id, ModifiedPlayer player);
		Task<Player> DeletePlayer (Guid id);
		Task IncrementScorePlayer(Guid id, PlayerScoreIncrement scoreIncrement);
		Task NameChangePlayer (Guid id, PlayerNameUpdate nameChange);
		Task<int> GetAverageScoreForPlayersBetweenDates(DateTime start, DateTime end);


		Task<Item> GetItem (Guid playerId, Guid itemId);
		Task<Item[]> GetAllItems (Guid playerId);
		Task<Item> CreateItem (Guid playerId, Item item);
		Task<Item> ModifyItem (Guid playerId, Guid itemId, ModifiedItem item);
		Task<Item> DeleteItem (Guid playerId, Guid itemId);
	}
}
