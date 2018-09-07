using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase {

	    private readonly PlayersProcessor _processor;

		public PlayersController (PlayersProcessor processor) {
			this._processor = processor;
		}

		// GET: api/players
		[HttpGet]
        public Task<Player[]> GetAll() {
            return _processor.GetAll();
        }

		// GET: api/players/0cda0b7c-df80-4126-b3ce-26f23cab7e54
		[HttpGet("{id}", Name = "GetPlayer")]
        public Task<Player> Get (Guid id)
        {
            return _processor.Get(id);
        }

		// POST: api/players
		[HttpPost]
        public Task<Player> Create ([FromBody] NewPlayer value) {
	        return _processor.Create(value);
        }

		// PUT: api/players/0cda0b7c-df80-4126-b3ce-26f23cab7e54
		[HttpPut("{id}")]
        public Task<Player> Modify (Guid id, [FromBody] ModifiedPlayer value) {
	        return _processor.Modify(id, value);
        }

		// DELETE: api/players/0cda0b7c-df80-4126-b3ce-26f23cab7e54
		[HttpDelete("{id}")]
        public Task<Player> Delete (Guid id) {
	        return _processor.Delete(id);
        }
    }
}
