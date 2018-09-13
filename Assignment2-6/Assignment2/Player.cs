using System;
using System.Collections.Generic;
using System.Text;
using Assignment2.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Assignment2 {

	public class Player {
		[BsonId]
		public Guid Id {
			get; set;
		}
		public string Name {
			get; set;
		}
		public int Level {
			get; set;
		}
		public int Score {
			get; set;
		}
		public bool IsBanned {
			get; set;
		}
		public DateTime CreationTime {
			get; set;
		}

		public List<Item> Items {
			get; set;
		}
	}

	public class NewPlayer {
		public string Name {
			get; set;
		}
	}

	public class ModifiedPlayer {
		public int? Score {
			get; set;
		}
		public int? Level {
			get; set;
		}
	}
}
