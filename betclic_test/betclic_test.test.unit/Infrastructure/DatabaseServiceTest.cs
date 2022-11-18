using betclic_test.Domain.Contracts;
using betclic_test.Infrastructure;
using betclic_test.Memory;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace betclic_test.test.unit.Infrastructure
{
    public class DatabaseServiceTest
    {
        private readonly IDatabaseService _databaseService;
        private readonly Mock<IBDD_Manager> bddManager;
        private readonly List<BDD_Player> players;


        public DatabaseServiceTest()
        {
            players = new List<BDD_Player>()
            { 
                new BDD_Player(1, "toto", 1)
                {
                    Score = 2,
                    TournamentPosition=1
                },
                new BDD_Player(2, "titi", 1)
                {
                    Score = 0,
                    TournamentPosition=2
                },
                new BDD_Player(3, "tutu", 1)
                {
                    Score = 0,
                    TournamentPosition=2
                },
            };

            bddManager = new Mock<IBDD_Manager>();
            bddManager.Setup(x => x.InsertPlayer(It.IsAny<string>(), It.IsAny<int?>())).Returns("default_tournament");
            bddManager.Setup(x => x.SelectPlayer("toto")).Returns(players[0]);
            bddManager.Setup(x => x.SelectPlayer("titi")).Returns(players[1]);
            bddManager.Setup(x => x.SelectPlayer("tutu")).Returns(players[2]);
            bddManager.Setup(x => x.GetTournamentId()).Returns(8);
            bddManager.Setup(x => x.GetTournamentId(It.IsAny<string>())).Returns(2);
            bddManager.Setup(x => x.DeletePlayersFromTournament(It.IsAny<int>())).Returns(1);
            bddManager.Setup(x => x.DeleteTournament(2)).Returns(true);
            bddManager.Setup(x => x.DeleteTournament(5)).Returns(false);
            bddManager.Setup(x => x.SelectTournamentPlayers()).Returns(players);
            _databaseService = new DatabaseService(bddManager.Object);
        }
        [Fact]
        public void ShouldDeletePlayersFromTournament()
        {
            var result = _databaseService.DeletePlayersFromTournament();
            result.ShouldBeTrue();
            bddManager.Verify(x => x.DeletePlayersFromTournament(8), Times.Once);
        }
        [Fact]
        public void ShouldAddPlayer()
        {
            var pseudo = "toto";
            var tournamentName = _databaseService.AddPlayer(pseudo);
            tournamentName.ShouldBe("default_tournament");
        }
        [Fact]
        public void ShouldGetPlayer()
        {
            var player = _databaseService.GetPlayer("toto");
            player.ShouldNotBeNull();
            player.ShouldBeEquivalentTo(PlayerMapper.Map(players[0]));
        }
        [Fact]
        public void ShouldUpdatePlayerScore()
        {
            var newScore = _databaseService.UpdatePlayerScore("toto", 8);
            newScore.ShouldBe(8);
            bddManager.Verify(x => x.UpdatePlayer(It.IsAny<int>(), It.IsAny<BDD_Player>()), Times.Once);
        }
        [Fact]
        public void ShouldAddPlayerNewPoints()
        {
            var newScore = _databaseService.AddPlayerPoints("toto", 4);
            newScore.ShouldBe(6);
            bddManager.Verify(x => x.UpdatePlayer(It.IsAny<int>(), It.IsAny<BDD_Player>()), Times.Once);
            bddManager.Verify(x => x.UpdatePlayerPositionInTournament(It.IsAny<int>()), Times.Once);
        }
        [Fact]
        public void ShouldGetPlayersSortedByScore()
        {
            var expectedPlayers = PlayerMapper.Map(players).ToList();
            var sortedPlayers = _databaseService.GetPlayersSortedByScore().ToList();
            sortedPlayers.ShouldNotBeNull();
            sortedPlayers.Count.ShouldBe(3);
            sortedPlayers.ShouldBeEquivalentTo(expectedPlayers);
        }
        [Fact]
        public void ShouldDeleteTournamentFromId()
        {
            var result = _databaseService.DeleteTournament(5);
            result.ShouldBeFalse();
            bddManager.Verify(x => x.GetTournamentId(It.IsAny<string>()), Times.Never);
            bddManager.Verify(x => x.DeletePlayersFromTournament(5), Times.Once);
            bddManager.Verify(x => x.DeleteTournament(5), Times.Once);
        }
        [Fact]
        public void ShouldDeleteTournamentFromName()
        {
            var result = _databaseService.DeleteTournament("default_tournament");
            result.ShouldBeTrue();
            bddManager.Verify(x => x.GetTournamentId(It.IsAny<string>()), Times.Once);
            bddManager.Verify(x => x.DeletePlayersFromTournament(2), Times.Once);
            bddManager.Verify(x => x.DeleteTournament(2), Times.Once);
        }
        [Fact]
        public void DeletePlayersFromTournament()
        {
            var result = _databaseService.DeletePlayersFromTournament();
            result.ShouldBeTrue();
            bddManager.Verify(x => x.GetTournamentId(), Times.Once);
            bddManager.Verify(x => x.DeletePlayersFromTournament(8), Times.Once);
        }
        [Fact]
        public void DeletePlayersFromTournamentFromId()
        {
            var result = _databaseService.DeletePlayersFromTournament(5);
            result.ShouldBeTrue();
            bddManager.Verify(x => x.GetTournamentId(It.IsAny<string>()), Times.Never);
            bddManager.Verify(x => x.DeletePlayersFromTournament(5), Times.Once);
        }
    }
}
