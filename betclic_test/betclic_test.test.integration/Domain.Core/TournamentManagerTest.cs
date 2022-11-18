using betclic_test.Domain.Contracts;
using betclic_test.Domain.Core;
using betclic_test.Infrastructure;
using betclic_test.Memory;
using Xunit;
using Shouldly;
using System.Collections.Generic;
using betclic_test.Domain.Models;
using System.Linq;
using System.Data.SQLite;
using System;

namespace betclic_test.test.integration.Domain.Core
{
    public class TournamentManagerTest
    {
        private readonly ITournamentManager tournamentManager;
        private readonly IDatabaseService databaseService;
        private readonly IBDD_Manager bddManager;
        private readonly List<Player> players;

        public TournamentManagerTest()
        {
            bddManager = new BDD_SQLite_Manager();
            databaseService = new DatabaseService(bddManager);
            tournamentManager = new TournamentManager(databaseService);

            players = new List<Player>()
            {
                new Player
                {
                    Pseudo = "riri",
                    Score = 12,
                    TournamentPosition = 2
                },
                new Player
                {
                    Pseudo = "fifi",
                    Score = 23,
                    TournamentPosition = 1
                },
                new Player
                {
                    Pseudo = "loulou",
                    Score = 11,
                    TournamentPosition = 3
                },
            };
        }

        [Fact]
        public void ShouldPlayTournamentWithoutException()
        {
            var playersSortedByScore = players.OrderByDescending(x => x.Score).ToList();
            var tname0 = tournamentManager.AddPlayer(players[0].Pseudo);
            var player0 = tournamentManager.GetPlayer(players[0].Pseudo);
            player0.ShouldNotBeNull();
            var tname1 = tournamentManager.AddPlayer(players[1].Pseudo);
            tname0.ShouldBe(tname1);
            var player1 = tournamentManager.GetPlayer(players[1].Pseudo);
            player0.ShouldNotBeNull();
            var tname2 = tournamentManager.AddPlayer(players[2].Pseudo);
            tname0.ShouldBe(tname2);
            var player2 = tournamentManager.GetPlayer(players[2].Pseudo);
            player0.ShouldNotBeNull();

            var scoreTable0 = tournamentManager.GetPlayersSortedByScore().ToList();
            scoreTable0.Count.ShouldBe(3);
            scoreTable0.All(x => x.TournamentPosition == 1).ShouldBeTrue();

            tournamentManager.UpdatePlayerScore(players[0].Pseudo, players[0].Score);
            tournamentManager.UpdatePlayerScore(players[1].Pseudo, players[1].Score);
            tournamentManager.UpdatePlayerScore(players[2].Pseudo, players[2].Score - 3);
            tournamentManager.AddPlayerPoints(players[2].Pseudo, 3);

            var scoreTable1 = tournamentManager.GetPlayersSortedByScore().ToList();
            scoreTable1.Count.ShouldBe(3);
            scoreTable1[0].Score.ShouldBe(playersSortedByScore[0].Score);
            scoreTable1[0].TournamentPosition.ShouldBe(playersSortedByScore[0].TournamentPosition);
            scoreTable1[0].Pseudo.ShouldBe(playersSortedByScore[0].Pseudo);
            scoreTable1[1].Score.ShouldBe(playersSortedByScore[1].Score);
            scoreTable1[1].TournamentPosition.ShouldBe(playersSortedByScore[1].TournamentPosition);
            scoreTable1[1].Pseudo.ShouldBe(playersSortedByScore[1].Pseudo);
            scoreTable1[2].Score.ShouldBe(playersSortedByScore[2].Score);
            scoreTable1[2].TournamentPosition.ShouldBe(playersSortedByScore[2].TournamentPosition);
            scoreTable1[2].Pseudo.ShouldBe(playersSortedByScore[2].Pseudo);

            var result = tournamentManager.DeletePlayersFromTournament();
            result.ShouldBeTrue();

            var scoreTable2 = tournamentManager.GetPlayersSortedByScore().ToList();
            scoreTable2.ShouldBeEmpty();

            Should.Throw<BDDException>(() => tournamentManager.GetPlayer(players[0].Pseudo));
            Should.Throw<BDDException>(() => tournamentManager.GetPlayer(players[1].Pseudo));
            Should.Throw<BDDException>(() => tournamentManager.GetPlayer(players[2].Pseudo));
        }

        [Fact]
        public void ShouldNotDeleteNonExistingTournament()
        {
            Should.Throw<BDDException>(() => tournamentManager.DeleteTournament("toto tournament"));
        }

        [Fact]
        public void ShouldNotGetNonExistingPlayers()
        {
            Should.Throw<BDDException>(() => tournamentManager.GetPlayer("NotExistingPseudo"));
        }
    }
}
