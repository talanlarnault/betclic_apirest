using betclic_test.Domain.Contracts;
using betclic_test.Domain.Models;
using betclic_test.Infrastructure;
using System.Collections.Generic;

namespace betclic_test.Domain.Core
{
    public class TournamentManager : ITournamentManager
    {
        private readonly IDatabaseService _databaseService;
        public TournamentManager(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        public int AddTournament(string name)
        {
            return _databaseService.AddTournament(name);
        }
        public string AddPlayer(string pseudo)
        {
            return _databaseService.AddPlayer(pseudo);
        }

        public double AddPlayerPoints(string pseudo, double newPoints)
        {
            return _databaseService.AddPlayerPoints(pseudo, newPoints);
        }

        public double UpdatePlayerScore(string pseudo, double newScore)
        {
            return _databaseService.UpdatePlayerScore(pseudo, newScore);
        }
        public bool DeleteTournament(string name)
        {
            return _databaseService.DeleteTournament(name);
        }
        public bool DeleteTournament(int id)
        {
            return _databaseService.DeleteTournament(id);
        }
        public bool DeletePlayersFromTournament(int tournamentId)
        {
            return _databaseService.DeletePlayersFromTournament(tournamentId);
        }
        public bool DeletePlayersFromTournament()
        {
            return _databaseService.DeletePlayersFromTournament();
        }
        public Player GetPlayer(string pseudo)
        {
            return _databaseService.GetPlayer(pseudo);
        }
        public IEnumerable<Player> GetPlayersSortedByScore()
        {
            return _databaseService.GetPlayersSortedByScore();
        }
    }
}
