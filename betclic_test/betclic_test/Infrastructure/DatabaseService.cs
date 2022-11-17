using betclic_test.Domain.Contracts;
using betclic_test.Domain.Models;
using betclic_test.Memory;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

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
            return _bddManager.InsertPlayer(pseudo);
        }

        private (int, string) GetLastTournament()
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand(); 
            sqlite_cmd.CommandText = "SELECT Id, Name From Tournament ORDER BY Id DESC LIMIT 1";

            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            sqlite_datareader.Read();
            return (sqlite_datareader.GetInt32(0), sqlite_datareader.GetString(1));
        }

        public double AddPlayerPoints(string pseudo, double newPoints)
        {
            var player = _bddManager.SelectPlayer(pseudo);
            player.Score += newPoints;
            _bddManager.UpdatePlayer(player.PlayerId, player);
            return player.Score;
        }

        public double UpdatePlayerScore(string pseudo, double newScore)
        {
            var player = _bddManager.SelectPlayer(pseudo);
            player.Score = newScore;
            _bddManager.UpdatePlayer(player.PlayerId, player);
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
            var myBddManager = _bddManager;
            var tournamentId = myBddManager.GetTournamentId();
            return myBddManager.DeletePlayersFromTournament(tournamentId) > 0;
        }
    }
}
