using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2_6.Assignment5;
using Assignment5;
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
		[HttpGet("{id:Guid}", Name = "GetPlayerById")]
        public Task<Player> GetById (Guid id)
        {
            return _processor.Get(id);
        }

	    // GET: api/players/kissa
	    [HttpGet("{name}", Name = "GetPlayerByName")]
	    public Task<Player> GetByName (string name) {
		    Filter<Player, string> filter = new EqFilter<Player, string>("Name", name);
			return _processor.Get(filter);
	    }
		/*
	    // GET: api/players?name=kissa
	    [HttpGet(Name = "GetPlayerByName")]
	    public Task<Player> GetPlayerByName ([FromQuery] string name) {
		    Filter<Player, string> filter = new EqFilter<Player, string>("Name", name);
		    return _processor.Get(filter);
	    }*/

		// GET: api/players?minScore=10
		[HttpGet("score", Name = "GetPlayersMinScore")]
	    public Task<Player[]> GetAllByMinScore ([FromQuery] int minScore) {
		    Filter<Player, int> filter = new MoreThanFilter<Player, int>("Score", minScore-1);
			return _processor.GetAll(filter);
	    }

	    // GET: api/players/top/10
	    [HttpGet("top/{number}", Name = "GetTopPlayers")]
	    public Task<Player[]> TopAmount (int number) {
		    return _processor.GetPlayersByScoreDescending(number);
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

	    // PUT: api/players/0cda0b7c-df80-4126-b3ce-26f23cab7e54
		[HttpPut("{id}/score")]
		public IActionResult IncrementScore (Guid id, [FromBody] PlayerScoreIncrement scoreIncrement) {
			_processor.IncrementScore(id, scoreIncrement);
			return Ok();
		}

	    // PUT: api/players/0cda0b7c-df80-4126-b3ce-26f23cab7e54
		[HttpPut("{id}/name")]
	    public IActionResult ChangeName (Guid id, [FromBody] PlayerNameUpdate nameUpdate) {
			_processor.ChangeName(id, nameUpdate);
		    return Ok();
		}


	    [HttpGet("score/avg/{start}/{end}", Name="AverageScoreBetweenDates")]
		public Task<int> AverageScoreBetweenDates (DateTime start, DateTime end) {
		    return _processor.AverageScoreBetweenDates(new BetweenDatesSelector() {
				startDate = start,
				endDate =  end
		    });
	    }
	}
}
