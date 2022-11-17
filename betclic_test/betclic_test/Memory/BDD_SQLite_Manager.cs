using betclic_test.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace betclic_test.Memory
{
    public class BDD_SQLite_Manager : IBDD_Manager
    {
        private SQLiteConnection sqlite_conn;
        public BDD_SQLite_Manager()
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

            string Createsql = "CREATE TABLE Tournament (Id INTEGER PRIMARY KEY ASC, Name VARCHAR(50))";
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

            string Createsql = "CREATE TABLE Player (Id INTEGER PRIMARY KEY ASC, Pseudo VARCHAR(50), TournamentId INTEGER, Score REAL DEFAULT 0, TournamentPosition INTEGER DEFAULT 999, FOREIGN KEY(TournamentId) REFERENCES Tournament(Id))";
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }
        public int DeletePlayersFromTournament(int tournamentId)
        {
            try
            {
                var tournamentPlayers = SelectTournamentPlayers(tournamentId);
                var idsToRemove = tournamentPlayers.Select(x => x.PlayerId);
                foreach (int idToRemove in idsToRemove) DeletePlayer(idToRemove);
                return idsToRemove.Count();
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to delete players of tounrament id : [{tournamentId}]", ex);
            }
        }
        private void DeletePlayer(int id)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = $"DELETE FROM Player WHERE Id={id};";
            sqlite_cmd.ExecuteNonQuery();
        }

        public bool DeleteTournament(int tournamentId)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = $"DELETE FROM Tournament WHERE Id={tournamentId};";
                sqlite_cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to delete players of tounrament id : [{tournamentId}]", ex);
            }
        }

        public int GetTournamentId()
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = $"SELECT Id FROM Tournament ORDER BY Id DESC LIMIT 1;";
                SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
                sqlite_datareader.Read();
                return sqlite_datareader.GetInt32(0);
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get tounament id", ex);
            }
        }

        public int GetTournamentId(string tournamentName)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = $"SELECT Id FROM Tournament WHERE Name='{tournamentName}' ORDER BY Id DESC LIMIT 1;";
                SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
                sqlite_datareader.Read();
                return sqlite_datareader.GetInt32(0);
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get tounament id from name [{tournamentName}]", ex);
            }
        }

        public string InsertPlayer(string pseudo, int? tournamentId = null)
        {
            (int tId, string tName) = getTournament(tournamentId);

            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Player (Pseudo, TournamentId) VALUES('{pseudo}', {tId});";
            sqlite_cmd.ExecuteNonQuery();

            this.UpdatePlayerPositionInTournament(tId);

            return tName;
        }

        public int InsertTournament(string name)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Tournament (Name) VALUES('{name}');";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "SELECT last_insert_rowid()";

            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            sqlite_datareader.Read();
            return sqlite_datareader.GetInt32(0);
        }

        public BDD_Player SelectPlayer(string pseudo)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = $"SELECT Id, Pseudo, TournamentId, Score, TournamentPosition FROM Player WHERE Pseudo='{pseudo}';";
                SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
                sqlite_datareader.Read();
                var player = new BDD_Player(sqlite_datareader.GetInt32(0), sqlite_datareader.GetString(1), sqlite_datareader.GetInt32(2))
                {
                    Score = sqlite_datareader.GetDouble(3),
                    TournamentPosition = sqlite_datareader.GetInt32(4)
                };
                return player;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get player from pseudo [{pseudo}]", ex);
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
                var players = new List<BDD_Player>();
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = $"SELECT Id, Pseudo, TournamentId, Score, TournamentPosition FROM Player WHERE TournamentId='{tournamentId}' ORDER BY Score DESC;";
                SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    players.Add(new BDD_Player(sqlite_datareader.GetInt32(0), sqlite_datareader.GetString(1), sqlite_datareader.GetInt32(2))
                    {
                        Score = sqlite_datareader.GetDouble(3),
                        TournamentPosition = sqlite_datareader.GetInt32(4)
                    });
                }

                return players;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to get players from tournament id : {tournamentId}", ex);
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

                return 1;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to update player [{player.Pseudo}({player.PlayerId})]", ex);
            }
        }

        public void UpdatePlayerPositionInTournament(int tournamentId)
        {
            var players = SelectTournamentPlayers(tournamentId).ToList();
            for (int i = 0; i < players.Count; i++)
            {
                int position = i+1;
                if (i == 0 || players[i].Score < players[i - 1].Score)
                {
                    if (players[i].TournamentPosition == position) continue;
                    players[i].TournamentPosition = position;
                }
                else
                {
                    if (players[i].TournamentPosition == players[i - 1].TournamentPosition) continue;
                    players[i].TournamentPosition = players[i - 1].TournamentPosition;
                }
                UpdatePlayer(players[i].PlayerId, players[i]);
            }
        }

        private (int, string) getTournament(int? tournamentId)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            if (getTournamentCount() == 0)
            {
                InsertTournament("default_tournament");
            }

            if (tournamentId.HasValue)
                sqlite_cmd.CommandText = $"SELECT Id, Name From Tournament WHERE Id={tournamentId.Value} ORDER BY Id DESC LIMIT 1";
            else
                sqlite_cmd.CommandText = $"SELECT * From Tournament";// ORDER BY Id DESC LIMIT 1";

            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            sqlite_datareader.Read();
            return (sqlite_datareader.GetInt32(0), sqlite_datareader.GetString(1));
        }

        private int getTournamentCount()
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT Count(Id) From Tournament";
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            sqlite_datareader.Read();
            return sqlite_datareader.GetInt32(0);
        }
    }
}
