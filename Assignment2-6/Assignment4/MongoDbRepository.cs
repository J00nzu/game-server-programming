﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2;
using Assignment2.Models;
using Assignment5;
using MongoDB.Driver;

namespace Assignment2_6.Assignment4
{
    public class MongoDbRepository : IRepository {

	    private MongoClient _client;
	    private IMongoDatabase _db;
	    private MongoCollectionBase<Player> _players;

	    private const string playerCollection = "Players";
	    private const string dbName = "game";

		public MongoDbRepository (string connectionString) {
			_client = new MongoClient(connectionString);
			_db = _client.GetDatabase(dbName);
			if (!_db.ListCollectionNames().ToList().Contains(playerCollection)) {
				_db.CreateCollection(playerCollection);
			}
			_players = _db.GetCollection<Player>(playerCollection) as MongoCollectionBase<Player>;
		}

	    public async Task<Player> GetPlayer(Guid id) {
		    var filter = Builders<Player>.Filter.Eq("Id", id);
			var result = await _players.FindAsync(filter);
		    return result.Single();
	    }

	    public async Task<Player> GetPlayer<T> (Filter<Player, T> filter) {
		    var result = await _players.FindAsync(filter.GetMongoDbFilter());
		    return result.First();
	    }

		public async Task<Player[]> GetAllPlayers() {
		    var filter = Builders<Player>.Filter.Empty;
		    var result = await _players.FindAsync(filter);

		    return result.ToList().ToArray();
		}

		public async Task<Player[]> GetAllPlayers<T> (Filter<Player, T> filter) {
			var result = await _players.FindAsync(filter.GetMongoDbFilter());
			return result.ToList().ToArray();
		}

		public async Task<Player[]> GetPlayersSortedByScore (int num, SortOrder order) {

			var filter = Builders<Player>.Filter.Empty;
			SortDefinition<Player> sortBy;

			switch (order) {
				case SortOrder.Ascending:
					sortBy = Builders<Player>.Sort.Ascending("Score");
					break;
				case SortOrder.Descending:
					sortBy = Builders<Player>.Sort.Descending("Score");
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(order), order, null);
			}

			var list = await _players.Find(filter).Sort(sortBy).Limit(num).ToListAsync();
			return list.ToArray();
		}


		public async Task<Player> CreatePlayer(Player player) {
			await _players.InsertOneAsync(player);
		    return player;
	    }

	    public async Task<Player> ModifyPlayer(Guid id, ModifiedPlayer player) {
		    var filter = Builders<Player>.Filter.Eq("Id", id);
		    UpdateDefinition<Player> LevelUpdate = null;
		    UpdateDefinition<Player> ScoreUpdate = null;
		    if (player.Level != null) {
			    LevelUpdate = Builders<Player>.Update.Set("Level", (int)player.Level);
		    }
		    if (player.Score != null) {
			    ScoreUpdate = Builders<Player>.Update.Set("Score", (int)player.Score);
		    }

		    var update = CombineCheckNull(LevelUpdate, ScoreUpdate);

			if(update == null) return (await _players.FindAsync(filter)).Single();

			await _players.FindOneAndUpdateAsync(filter, update); // returning this would return the before update version
			return (await _players.FindAsync(filter)).Single(); // so we have to return this
		}

	    public async Task<Player> DeletePlayer(Guid id) {
			var filter = Builders<Player>.Filter.Eq("Id", id);

			return await _players.FindOneAndDeleteAsync(filter);
		}

	    public async Task IncrementScorePlayer(Guid id, PlayerScoreIncrement scoreIncrement) {
			var filter = Builders<Player>.Filter.Eq("Id", id);
		    var update = Builders<Player>.Update.Inc("Score", scoreIncrement.ScoreIncrement);

		    await _players.UpdateOneAsync(filter, update);
	    }

	    public async Task NameChangePlayer(Guid id, PlayerNameUpdate nameChange) {
			var filter = Builders<Player>.Filter.Eq("Id", id);
		    var update = Builders<Player>.Update.Set("Name", nameChange.Name);

		    await _players.UpdateOneAsync(filter, update);
		}

	    public async Task<int> GetAverageScoreForPlayersBetweenDates(DateTime start, DateTime end) {
			/*
		    var aggregate = _players.Aggregate()
			    .Match(x => x.CreationTime > start)
			    .Match(x => x.CreationTime < end);
		    double avg = aggregate.ToList().AsQueryable().Average(x => x.Score);
			*/
		    try {
			    var aggregate = await _players.Aggregate()
				    .Match(x => x.CreationTime > start)
				    .Match(x => x.CreationTime < end)
				    .Group(x => 0 /* equal to group by nothing. Couldn't find another way to do this.*/
					    , g => new {
					    avg = g.Average(s => s.Score)
				    }).ToCursorAsync();

			    var avg = aggregate.FirstOrDefault().avg;

			    return (int) avg;
		    }
		    catch (Exception ex) {
				Console.WriteLine(ex.Message);
			    return 0;
		    }
		    
	    }

	    public async Task<Item> GetItem(Guid playerId, Guid itemId) {
			var filter = Builders<Player>.Filter.Eq("Id", playerId);
		    var player = (await _players.FindAsync(filter)).Single();
		    return GetItemByID(player, itemId);
	    }

	    public async Task<Item[]> GetAllItems(Guid playerId) {
			var filter = Builders<Player>.Filter.Eq("Id", playerId);
		    var player = (await _players.FindAsync(filter)).Single();
		    return player.Items.ToArray();
	    }

	    public async Task<Item> CreateItem(Guid playerId, Item item) {
			var filter = Builders<Player>.Filter.Eq("Id", playerId);
		    var update = Builders<Player>.Update.AddToSet("Items", item);
		    await _players.FindOneAndUpdateAsync(filter, update);
		    return item;
	    }

	    public async Task<Item> ModifyItem(Guid playerId, Guid itemId, ModifiedItem item) {
			//lambda magic roughly translates to:
			//find a player where ids match,
			//inside wich find an item where ids match,
			//then update the found item Level
			var filter = Builders<Player>.Filter.Where(x => x.Id == playerId && x.Items.Any(it => it.Id == itemId));
		    var update = Builders<Player>.Update.Set(x => x.Items[-1].Level, item.Level);


		    var player  = await _players.FindOneAndUpdateAsync(filter, update);

		    var i = GetItemByID(player, itemId);
		    i.Level = item.Level; // to show the player the updated state

		    return i;
	    }

	    public async Task<Item> DeleteItem(Guid playerId, Guid itemId) {
			var filter = Builders<Player>.Filter.Eq("Id", playerId);
		    var itemFilter = Builders<Item>.Filter.Eq("Id", itemId);
		    var update = Builders<Player>.Update.PullFilter("Items", itemFilter);

		    var player = await _players.FindOneAndUpdateAsync(filter, update);
		    return GetItemByID(player, itemId);
		}

		private UpdateDefinition<T> CombineCheckNull<T> (params UpdateDefinition<T>[] defs) {
			if (defs.Length == 0) {
				return null;
			}

			UpdateDefinition<T> update = null;
			foreach (var definition in defs) {
				if (update == null) {
					if (definition == null)
						continue;
					update = definition;
				} else if(definition != null) {
					update = Builders<T>.Update.Combine(update, definition);
				}

			}

			return update;
		}

	    private Item GetItemByID (Player player, Guid itemId) {
		    foreach (Item playerItem in player.Items) {
			    if (playerItem.Id == itemId) {
				    return playerItem;
			    }
		    }

		    return null;
		}
	}
}
