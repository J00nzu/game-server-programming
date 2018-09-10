using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2 {
	public class InvalidPlayerLevelException : Exception {

		public InvalidPlayerLevelException(string msg)  : base(msg) { }
	}
}
