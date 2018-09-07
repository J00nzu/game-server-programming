using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Controllers;
using Assignment2.Processors;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment2
{
    public static class StaticExtensions {

		public static IServiceCollection UseRepository (this IServiceCollection service, IRepository rep) {
			return service.AddSingleton(rep);
		}

		public static IServiceCollection UsePlayersProcessor (this IServiceCollection service) {
		    return service.AddSingleton<PlayersProcessor>();
	    }

	    public static IServiceCollection UseItemsProcessor (this IServiceCollection service) {
		    return service.AddSingleton<ItemsProcessor>();
	    }
	}
}
