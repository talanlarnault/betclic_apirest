using betclic_test.Domain.Models;
using betclic_test.Memory;
using System.Collections.Generic;
using System.Linq;

namespace betclic_test.Domain.Contracts
{
    public static class PlayerMapper
    {
        public static Player Map(BDD_Player bdd_player)
        {
            return new Player
            {
                Pseudo = bdd_player.Pseudo,
                Score = bdd_player.Score,
                TournamentId = bdd_player.TournamentId,
                TournamentPosition = bdd_player.TournamentPosition
            };
        }
        public static IEnumerable<Player> Map(IEnumerable<BDD_Player> bdd_players)
        {
            return bdd_players.Select(x => Map(x));
        }
    }
}
