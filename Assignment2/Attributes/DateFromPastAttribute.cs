using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Attributes {
	public class DateFromPastAttribute : ValidationAttribute {

		protected override ValidationResult IsValid (object value, ValidationContext context) {
			// should account for any Time desync between server and client
			DateTime currentTime = DateTime.Now + TimeSpan.FromMinutes(10); 

			DateTime date = (DateTime)value;

			if (date > currentTime) {
				return new ValidationResult("Given date is from the future! this is not allowed.");
			}

			return ValidationResult.Success;
		}
	}
}
