using betclic_test.Domain.Contracts;
using betclic_test.Domain.Models;
using System;
using System.Collections.Generic;

namespace betclic_test.Infrastructure
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IBDD_Manager _bddManager;

        public IServiceProvider ServiceProvider { get; }

        public DatabaseService(IBDD_Manager bddManager)
        {
            _bddManager = bddManager;
        }

        public int AddTournament(string name)
        {
            return _bddManager.InsertTournament(name);
        }

        public string AddPlayer(string pseudo)
        {
            var tournamentName = _bddManager.InsertPlayer(pseudo);
            return tournamentName;
        }

        public double AddPlayerPoints(string pseudo, int newPoints)
        {
            var player = _bddManager.SelectPlayer(pseudo);
            player.Score += newPoints;
            _bddManager.UpdatePlayer(player.PlayerId, player);
            _bddManager.UpdatePlayerPositionInTournament(player.TournamentId);
            return player.Score;
        }

        public double UpdatePlayerScore(string pseudo, int newScore)
        {
            var player = _bddManager.SelectPlayer(pseudo);
            player.Score = newScore;
            _bddManager.UpdatePlayer(player.PlayerId, player);
            _bddManager.UpdatePlayerPositionInTournament(player.TournamentId);
            return player.Score;
        }

        public IEnumerable<Player> GetPlayersSortedByScore()
        {
            var players = _bddManager.SelectTournamentPlayers();
            return PlayerMapper.Map(players);
        }
        public Player GetPlayer(string pseudo)
        {
            var player = _bddManager.SelectPlayer(pseudo);
            return PlayerMapper.Map(player);
        }
        public bool DeleteTournament(string name)
        {
            var id = _bddManager.GetTournamentId(name);
            _bddManager.DeletePlayersFromTournament(id);
            return _bddManager.DeleteTournament(id);
        }
        public bool DeleteTournament(int id)
        {
            _bddManager.DeletePlayersFromTournament(id);
            return _bddManager.DeleteTournament(id);
        }
        public bool DeletePlayersFromTournament(int tournamentId)
        {
            return _bddManager.DeletePlayersFromTournament(tournamentId) > 0;
        }
        public bool DeletePlayersFromTournament()
        {
            var tournamentId = _bddManager.GetTournamentId();
            return _bddManager.DeletePlayersFromTournament(tournamentId) > 0;
        }
    }
}
