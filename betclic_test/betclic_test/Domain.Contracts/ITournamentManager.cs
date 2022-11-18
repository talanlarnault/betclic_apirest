using betclic_test.Domain.Models;
using System.Collections.Generic;

namespace betclic_test.Domain.Contracts
{
    public interface ITournamentManager
    {
        string AddPlayer(string pseudo);
        double AddPlayerPoints(string pseudo, int newPoints);
        int AddTournament(string name);
        bool DeletePlayersFromTournament(int tournamentId);
        bool DeletePlayersFromTournament();
        bool DeleteTournament(string name);
        bool DeleteTournament(int id);
        Player GetPlayer(string pseudo);
        IEnumerable<Player> GetPlayersSortedByScore();
        double UpdatePlayerScore(string pseudo, int newScore);
    }
}