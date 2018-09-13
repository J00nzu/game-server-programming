using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Attributes;

namespace Assignment2.Models {
	public enum ItemType {
		Sword,
		Axe,
		Armor,
		Shield,
		Potion,
		Herb
		/*...*/
	}

	public class Item {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public ItemType Type { get; set; }
		public DateTime CreationDate { get; set; }
	}

	public class NewItem {
		[StringLength(128)]
		public string Name { get; set; }

		[Range(1,99)]
		public int Level {
			get; set;
		}

		[AllowedItemTypes(ItemType.Armor, ItemType.Sword, ItemType.Axe, ItemType.Herb, ItemType.Potion, ItemType.Shield)]
		public ItemType Type {
			get; set;
		}

		[DateFromPast]
		[DataType(DataType.Date)]
		public DateTime CreationDate {
			get; set;
		}
	}

	public class ModifiedItem {
		[Range(1, 99)]
		public int Level { get; set; }
	}
}
