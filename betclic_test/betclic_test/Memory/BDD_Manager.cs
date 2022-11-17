using betclic_test.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace betclic_test.Memory
{
    public class BDD_Manager : IBDD_Manager
    {
        private int nextBddTournamentId = 0;
        private int nextBddPlayerId = 0;
        public Dictionary<int, BDD_Tournament> Tournaments = new();
        public Dictionary<int, BDD_Player> Players = new();

        private SQLiteConnection sqlite_conn;

        public BDD_Manager()
        {

            CreateConnection();
            CreateTables();
        }
        private SQLiteConnection CreateConnection()
        {
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True;");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sqlite_conn;
        }
        private void CreateTables()
        {
            CreateTournamentTable();
            CreatePlayerTable();
        }
        private void CreateTournamentTable()
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT name FROM sqlite_master WHERE name='Tournament'";
            var name = sqlite_cmd.ExecuteScalar();
            // check account table exist or not 
            // if exist do nothing 
            if (name != null && name.ToString() == "Tournament")
            {
                string Deletesql = "DROP TABLE Tournament";
                sqlite_cmd.CommandText = Deletesql;
                sqlite_cmd.ExecuteNonQuery();
                //return;
            }

            string Createsql = "CREATE TABLE Tournament (Id INT PRIMARY KEY ASC, Name VARCHAR(50))";
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }
        private void CreatePlayerTable()
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT name FROM sqlite_master WHERE name='Player'";
            var name = sqlite_cmd.ExecuteScalar();
            // check account table exist or not 
            // if exist do nothing 
            if (name != null && name.ToString() == "Player")
            {
                string Deletesql = "DROP TABLE Player";
                sqlite_cmd.CommandText = Deletesql;
                sqlite_cmd.ExecuteNonQuery();
                //return;
            }

            string Createsql = "CREATE TABLE Player (Id INT PRIMARY KEY ASC, Pseudo VARCHAR(50), TournamentId INT, Score REAL, TournamentPosition INT, FOREIGN KEY(TournamentId) REFERENCES Tournament(Id))";
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public int InsertTournament(string name)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Tournament (Name) VALUES('{name}');";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "SELECT last_insert_rowid()";

            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                var myreader = sqlite_datareader.GetInt32(0);
                Console.WriteLine(myreader);
            }

            if (Tournaments.Values.Any(x => x.TournamentName.Equals(name, System.StringComparison.OrdinalIgnoreCase))) throw new BDDException("Tournament name already exists.");
            var newId = nextBddTournamentId++;
            var newTournament = new BDD_Tournament(newId, name);
            if (Tournaments.TryAdd(newId, newTournament) == false) throw new BDDException("An unexpected error happened during tournament creation.");
            return newId;
        }
        public string InsertPlayer(string pseudo, int? tournamentId = null)
        {
            (int tId, string tName) = getTournament(tournamentId);

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Player (Pseudo, TournamentId) VALUES('{pseudo}', {tId});";
            sqlite_cmd.ExecuteNonQuery();
            Console.WriteLine(tName);

            var newId = nextBddPlayerId++;
            int tournamentIdToUse = getTournamentId(tournamentId);
            if (Players.Values.Any(x => x.Pseudo.Equals(pseudo, System.StringComparison.OrdinalIgnoreCase) && x.TournamentId == tournamentIdToUse)) throw new BDDException("Player is already registered on this tournament.");
            var newPlayer = new BDD_Player(newId, pseudo, tournamentIdToUse);
            if (Players.TryAdd(newId, newPlayer) == false) throw new BDDException("An unexpected error happened during player registration.");

            return getTournamentName(tournamentIdToUse);
        }

        public BDD_Player SelectPlayer(string pseudo)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = $"SELECT Id, Pseudo, TournamentId, Score, TournamentPosition FROM Player WHERE Pseudo='{pseudo}';";
                sqlite_cmd.ExecuteReader();
                SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
                sqlite_datareader.Read();
                var player = new BDD_Player(sqlite_datareader.GetInt32(0), sqlite_datareader.GetString(1), sqlite_datareader.GetInt32(2))
                {
                    Score = sqlite_datareader.GetDouble(3),
                    TournamentPosition = sqlite_datareader.GetInt32(4)
                };
                Console.WriteLine(player.Pseudo);

                return Players.Values.Where(x => x.Pseudo == pseudo).Last();
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get player from pseudo [{pseudo}]", ex);
            }
        }
        public int UpdatePlayer(int id, BDD_Player player)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = $"UPDATE Player SET Score={player.Score}, TournamentPosition={player.TournamentPosition} WHERE Id={id};";
                sqlite_cmd.ExecuteNonQuery();

                Players[id] = player;
                UpdatePlayerPositionInTournament(player.TournamentId);
                return 1;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to update player [{player.Pseudo}({player.PlayerId})]", ex);
            }
        }
        private void UpdatePlayerPositionInTournament(int tournamentId)
        {
            var positions = Players.Values.Where(x => x.TournamentId == tournamentId).OrderByDescending(x => x.Score).ToList();
            for (int i = 0; i < positions.Count(); i++)
            {
                int position = i;
                if (i == 0 || positions[i].Score < positions[i - 1].Score)
                {
                    positions[i].TournamentPosition = position;
                }
                else
                    positions[i].TournamentPosition = positions[i - 1].TournamentPosition;
            }
        }
        public IEnumerable<BDD_Player> SelectTournamentPlayers()
        {
            return SelectTournamentPlayers(GetTournamentId());
        }
        public IEnumerable<BDD_Player> SelectTournamentPlayers(int tournamentId)
        {
            try
            {
                return Players.Values.Where(x => x.TournamentId == tournamentId).OrderByDescending(x => x.Score);
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get players from tournament id : {tournamentId}", ex);
            }
        }
        public int DeletePlayersFromTournament(int tournamentId)
        {
            try
            {
                var tournamentPlayers = SelectTournamentPlayers(tournamentId);
                var idsToRemove = tournamentPlayers.Select(x => x.PlayerId);
                var count = idsToRemove.Count();
                foreach (int idToRemove in idsToRemove) Players.Remove(idToRemove);
                return count;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to delete players of tounrament id : {tournamentId}]", ex);
            }
        }
        public bool DeleteTournament(int tournamentId)
        {
            try
            {
                return Tournaments.Remove(tournamentId);
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to delete players of tounrament id : {tournamentId}]", ex);
            }
        }
        private (int, string) getTournament(int? tournamentId)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT Count(Id) From Tournament";
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            sqlite_datareader.Read();
            if(sqlite_datareader.GetInt32(0) == 0)
            {
                InsertTournament("default_tournament");
            }
            if(tournamentId.HasValue)
                sqlite_cmd.CommandText = $"SELECT Id, Name From Tournament WHERE Id={tournamentId.Value} ORDER BY Id DESC LIMIT 1";
            else
                sqlite_cmd.CommandText = $"SELECT Id, Name From Tournament ORDER BY Id DESC LIMIT 1";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            sqlite_datareader.Read();
            return (sqlite_datareader.GetInt32(0), sqlite_datareader.GetString(1));
        }
        private int getTournamentId(int? tournamentId)
        {
            if (Tournaments.Keys.Any() == false) InsertTournament("default_tournament");
            var tournamentIdToUse = tournamentId.HasValue ? tournamentId.Value : Tournaments.Keys.Last();
            return tournamentIdToUse;
        }

        public int GetTournamentId(string tournamentName)
        {
            try
            {
                return Tournaments.Values.First(x => x.TournamentName.Equals(tournamentName, StringComparison.OrdinalIgnoreCase)).TournamentId;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get tounament id from name [{tournamentName}]", ex);
            }
        }

        public int GetTournamentId()
        {
            try
            {
                return Tournaments.Last().Value.TournamentId;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get tounament id", ex);
            }
        }

        private string getTournamentName(int id)
        {
            try
            {
                return Tournaments[id].TournamentName;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get tounament name from id [{id}]", ex);
            }
        }
    }
}
