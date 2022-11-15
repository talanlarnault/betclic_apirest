using betclic_test.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace betclic_test.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class TournamentController : ControllerBase
    {
        public readonly ITournamentManager _tournamentManager;
        private readonly ILogger<TournamentController> _logger;

        public TournamentController(ILogger<TournamentController> logger, ITournamentManager tournamentManager)
        {
            _logger = logger;
            _tournamentManager = tournamentManager;
        }
        [HttpPut("{name}")]
        public IActionResult AddTournament(string name)
        {
            try
            {
                var id = _tournamentManager.AddTournament(name);
                return Ok($"Tournament [{name}] registered at id : {id}.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Tournament [{name}]not registered because an error occured : {ex.Message} !");
            }
        }
        [HttpPut("{pseudo}")]
        public IActionResult AddPlayer(string pseudo)
        {
            try
            {
                var tournamentName = _tournamentManager.AddPlayer(pseudo);
                return Ok($"Player [{pseudo}] registered on tournament [{tournamentName}]");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
        [HttpPut("{pseudo}/{newPoints}")]
        public IActionResult AddPlayerPoints(string pseudo, double newPoints)
        {
            try
            {
                var score = _tournamentManager.AddPlayerPoints(pseudo, newPoints);
                return Ok($"Player [{pseudo}] new score : [{score}]");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
        [HttpPut("{pseudo}/{newScore}")]
        public IActionResult UpdatePlayerScore(string pseudo, double newScore)
        {
            try
            {
                var score = _tournamentManager.UpdatePlayerScore(pseudo, newScore);
                return Ok($"Player [{pseudo}] new score : [{score}]");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteTournamentById(int id)
        {
            try
            {
                if (_tournamentManager.DeleteTournament(id)) return Ok($"Tournament deleted !");
                return StatusCode(500, $"Tournament not deleted !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
        [HttpDelete("{name}")]
        public IActionResult DeleteTournamentByName(string name)
        {
            try
            {
                if (_tournamentManager.DeleteTournament(name)) return Ok($"Tournament deleted !");
                return StatusCode(500, $"Tournament not deleted !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
        [HttpDelete]
        public IActionResult DeletePlayers()
        {
            try
            {
                if (_tournamentManager.DeletePlayersFromTournament()) return Ok($"Players deleted !");
                return StatusCode(500, $"No player was found to be deleted !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
        [HttpGet("{pseudo}")]
        public IActionResult GetPlayer(string pseudo)
        {
            try
            {
                var player = _tournamentManager.GetPlayer(pseudo);
                return Ok(player);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
        [HttpGet]
        public IActionResult GetScoreTable()
        {
            try
            {
                var players = _tournamentManager.GetPlayersSortedByScore();
                return Ok(players);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
    }
}
