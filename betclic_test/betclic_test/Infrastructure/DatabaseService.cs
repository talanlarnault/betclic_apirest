using betclic_test.Domain.Contracts;
using betclic_test.Domain.Models;
using betclic_test.Memory;
using System.Collections.Generic;

namespace betclic_test.Infrastructure
{
    public class DatabaseService : IDatabaseService
    {
        private BDD_Manager bddManager
        {
            get
            {
                return new BDD_Manager();
            }
        }
        public DatabaseService()
        {
        }

        public int AddTournament(string name)
        {
            return bddManager.InsertTournament(name);
        }

        public string AddPlayer(string pseudo)
        {
            return bddManager.InsertPlayer(pseudo);
        }

        public double AddPlayerPoints(string pseudo, double newPoints)
        {
            var myBddManager = bddManager;
            var player = myBddManager.SelectPlayer(pseudo);
            player.Score += newPoints;
            myBddManager.UpdatePlayer(player.PlayerId, player);
            return player.Score;
        }

        public double UpdatePlayerScore(string pseudo, double newScore)
        {
            var myBddManager = bddManager;
            var player = myBddManager.SelectPlayer(pseudo);
            player.Score = newScore;
            myBddManager.UpdatePlayer(player.PlayerId, player);
            return player.Score;
        }

        public IEnumerable<Player> GetPlayersSortedByScore()
        {
            var players = bddManager.SelectTournamentPlayers();
            return PlayerMapper.Map(players);
        }
        public Player GetPlayer(string pseudo)
        {
            var player = bddManager.SelectPlayer(pseudo);
            return PlayerMapper.Map(player);
        }
        public bool DeleteTournament(string name)
        {
            var myBddManager = bddManager;
            var id = myBddManager.GetTournamentId(name);
            myBddManager.DeletePlayersFromTournament(id);
            return myBddManager.DeleteTournament(id);
        }
        public bool DeleteTournament(int id)
        {
            var myBddManager = bddManager;
            myBddManager.DeletePlayersFromTournament(id);
            return myBddManager.DeleteTournament(id);
        }
        public bool DeletePlayersFromTournament(int tournamentId)
        {
            return bddManager.DeletePlayersFromTournament(tournamentId) > 0;
        }
        public bool DeletePlayersFromTournament()
        {
            var myBddManager = bddManager;
            var tournamentId = myBddManager.GetTournamentId();
            return myBddManager.DeletePlayersFromTournament(tournamentId) > 0;
        }
    }
}
