using betclic_test.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace betclic_test.Memory
{
    public class BDD_Manager : IBDD_Manager
    {
        private int nextBddTournamentId = 0;
        private int nextBddPlayerId = 0;
        public Dictionary<int, BDD_Tournament> Tournaments = new();
        public Dictionary<int, BDD_Player> Players = new();

        public int InsertTournament(string name)
        {
            if (Tournaments.Values.Any(x => x.TournamentName.Equals(name, System.StringComparison.OrdinalIgnoreCase))) throw new BDDException("Tournament name already exists.");
            var newId = nextBddTournamentId++;
            var newTournament = new BDD_Tournament(newId, name);
            if (Tournaments.TryAdd(newId, newTournament) == false) throw new BDDException("An unexpected error happened during tournament creation.");
            return newId;
        }
        public string InsertPlayer(string pseudo, int? tournamentId = null)
        {
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
                Players[id] = player;
                return 1;
            }
            catch (Exception ex)
            {
                throw new BDDException($"Impossible to update player [{player.Pseudo}({player.PlayerId})]", ex);
            }
        }
        public void UpdatePlayerPositionInTournament(int tournamentId)
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
