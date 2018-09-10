using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Assignment2 {

	public class ShowMessageExceptionFilterAttribute : ExceptionFilterAttribute {
		private Type _type;

		public ShowMessageExceptionFilterAttribute (Type exceptionType) {
			_type = exceptionType;
		}

		public override void OnException (ExceptionContext context) {
			/*if (!_hostingEnvironment.IsDevelopment()) {
				// do nothing
				return;
			}
			var result = new ViewResult { ViewName = "CustomError" };
			result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
			result.ViewData.Add("Exception", context.Exception);
			// TODO: Pass additional detailed data via ViewData
			context.Result = result;*/

			if (!(_type.IsAssignableFrom(context.Exception.GetType()))) {
				return;
			}
			JsonResult error = new JsonResult(context.Exception.Message);
			error.StatusCode = StatusCodes.Status422UnprocessableEntity;

			context.ExceptionHandled = true;
			context.Result = error;
		}
	}
}
