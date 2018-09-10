using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Models;
using Assignment2.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Assignment2.Controllers
{
    [Route("api/players/{playerId}/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase {
	    private readonly ItemsProcessor _processor;

		public ItemsController (ItemsProcessor processor) {
			_processor = processor;
		}

		// GET: api/players/{playerId}/items
		[HttpGet]
		public Task<Item[]> GetAll (Guid playerId) {
			return _processor.GetAll(playerId);
		}

		// GET: api/players/{playerId}/items/{itemId}
		[HttpGet("{itemId}", Name = "GetItem")]
        public Task<Item> Get(Guid playerId, Guid itemId) {
            return _processor.Get(playerId, itemId);
        }

		// POST: api/players/{playerId}/items
		[ShowMessageExceptionFilter(typeof(InvalidPlayerLevelException))]
        public Task<Item> Create (Guid playerId, [FromBody] NewItem value) {
	        return _processor.Create(playerId, value);
		}

		// PUT: api/players/{playerId}/items/{itemId}
		[HttpPut("{itemId}")]
		public Task<Item> Modify (Guid playerId, Guid itemId, [FromBody] ModifiedItem value) {
			return _processor.Modify(playerId, itemId, value);
		}

		// DELETE: api/players/{playerId}/items/{itemId}
		[HttpDelete("{itemId}")]
        public Task<Item> Delete (Guid playerId, Guid itemId) {
			return _processor.Delete(playerId, itemId);
		}
    }
}
