using betclic_test.Domain.Contracts;
using betclic_test.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace betclic_test
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        public ITournamentManager _tournamentManager { get; }

        public TournamentController(ITournamentManager tournamentManager)
        {
            _tournamentManager = tournamentManager;
        }
        [HttpPut]
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
        [HttpPut]
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
        [HttpPut]
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
        [HttpPut]
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
        [HttpDelete]
        public IActionResult DeleteTournament(int id)
        {
            try
            {
                if(_tournamentManager.DeleteTournament(id)) return Ok($"Tournament deleted !");
                return StatusCode(500, $"Tournament not deleted !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message} !");
            }
        }
        [HttpDelete]
        public IActionResult DeleteTournament(string name)
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
        [HttpGet]
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
