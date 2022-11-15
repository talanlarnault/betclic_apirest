using betclic_test.Domain.Contracts;
using betclic_test.Domain.Models;
using betclic_test.Infrastructure;
using System.Collections.Generic;

namespace betclic_test.Domain.Core
{
    public class TournamentManager : ITournamentManager
    {
        private readonly IDatabaseService databaseService;
        private TournamentManager()
        {
            databaseService = new DatabaseService();
        }
        public int AddTournament(string name)
        {
            return databaseService.AddTournament(name);
        }
        public string AddPlayer(string pseudo)
        {
            return databaseService.AddPlayer(pseudo);
        }

        public double AddPlayerPoints(string pseudo, double newPoints)
        {
            return databaseService.AddPlayerPoints(pseudo, newPoints);
        }

        public double UpdatePlayerScore(string pseudo, double newScore)
        {
            return databaseService.UpdatePlayerScore(pseudo, newScore);
        }
        public bool DeleteTournament(string name)
        {
            return databaseService.DeleteTournament(name);
        }
        public bool DeleteTournament(int id)
        {
            return databaseService.DeleteTournament(id);
        }
        public bool DeletePlayersFromTournament(int tournamentId)
        {
            return databaseService.DeletePlayersFromTournament(tournamentId);
        }
        public bool DeletePlayersFromTournament()
        {
            return databaseService.DeletePlayersFromTournament();
        }
        public Player GetPlayer(string pseudo)
        {
            return databaseService.GetPlayer(pseudo);
        }
        public IEnumerable<Player> GetPlayersSortedByScore()
        {
            return databaseService.GetPlayersSortedByScore();
        }
    }
}
