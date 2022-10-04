using System.Collections.Generic;
using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;
using Xunit;

namespace MockingUnitTestsDemoApp.Tests.MockingUnitTestsDemoApp.Impl.Services
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerRepository> _mockPlayerRepository = new Mock<IPlayerRepository>();
        private readonly Mock<ITeamRepository> _mockTeamRepository = new Mock<ITeamRepository>();
        private readonly Mock<ILeagueRepository> _mockLeagueRepository = new Mock<ILeagueRepository>();
        private readonly PlayerService _playerService;

        public PlayerServiceTests()
        {
            _playerService = new PlayerService(_mockPlayerRepository.Object,
                                               _mockTeamRepository.Object,
                                               _mockLeagueRepository.Object);
        }

        [Fact]
        public void NotEmptyPlayersListTest()
        {
            // Arrange
            _mockLeagueRepository.Setup(x => x.IsValid(It.IsAny<int>())).Returns(true);

            _mockTeamRepository.Setup(x => x.GetForLeague(It.IsAny<int>())).Returns(GetFakeTeamsList());
        
            _mockPlayerRepository.Setup(x => x.GetForTeam(It.IsAny<int>())).Returns(GeetFakePlayersList());

            // Act
            var playersList = _playerService.GetForLeague(It.IsAny<int>());

            // Assert
            playersList.Should().NotBeEmpty();

            _mockLeagueRepository.Verify(x => x.IsValid(It.IsAny<int>()), Times.Once);
            _mockTeamRepository.Verify(x => x.GetForLeague(It.IsAny<int>()), Times.Once);
            _mockPlayerRepository.Verify(x => x.GetForTeam(It.IsAny<int>()), Times.Exactly(GetFakeTeamsList().Count));
        }

        [Fact]
        public void EmptyPlayersListTest()
        {
            // Arrange
            _mockLeagueRepository.Setup(x => x.IsValid(It.IsAny<int>())).Returns(false);

            // Act
            var playersList = _playerService.GetForLeague(It.IsAny<int>());

            // Assert
            playersList.Should().BeEmpty();

            _mockLeagueRepository.Verify(x => x.IsValid(It.IsAny<int>()), Times.Once);
            _mockTeamRepository.Verify(x => x.GetForLeague(It.IsAny<int>()), Times.Never);
            _mockPlayerRepository.Verify(x => x.GetForTeam(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void EmptyTeamsListTest()
        {
            // Arrange
            _mockLeagueRepository.Setup(x => x.IsValid(It.IsAny<int>())).Returns(true);

            _mockTeamRepository.Setup(x => x.GetForLeague(It.IsAny<int>())).Returns(new List<Team>());

            // Act
            var playersList= _playerService.GetForLeague(It.IsAny<int>());

            // Assert
            playersList.Should().BeEmpty();

            _mockLeagueRepository.Verify(x => x.IsValid(It.IsAny<int>()), Times.Once);
            _mockTeamRepository.Verify(x => x.GetForLeague(It.IsAny<int>()), Times.Once);
            _mockPlayerRepository.Verify(x => x.GetForTeam(It.IsAny<int>()), Times.Never);
        }

        private List<Team> GetFakeTeamsList() 
            => new()
            {
                new Team { ID = 1 },
                new Team { ID = 2 },
                new Team { ID = 3 },
                new Team { ID = 4 },
                new Team { ID = 5 }
            };

        private List<Player> GeetFakePlayersList() 
            => new()
            {
                new Player { ID = 1, TeamID = 3 },
                new Player { ID = 2, TeamID = 3 }
            };
    }
}
