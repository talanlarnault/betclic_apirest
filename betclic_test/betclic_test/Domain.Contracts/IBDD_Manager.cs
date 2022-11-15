using betclic_test.Memory;
using System.Collections.Generic;

namespace betclic_test.Domain.Contracts
{
    public interface IBDD_Manager
    {
        int DeletePlayersFromTournament(int tournamentId);
        bool DeleteTournament(int tournamentId);
        int GetTournamentId();
        int GetTournamentId(string tournamentName);
        string InsertPlayer(string pseudo, int? tournamentId = null);
        int InsertTournament(string name);
        BDD_Player SelectPlayer(string pseudo);
        IEnumerable<BDD_Player> SelectTournamentPlayers();
        IEnumerable<BDD_Player> SelectTournamentPlayers(int tournamentId);
        int UpdatePlayer(int id, BDD_Player player);
    }
}