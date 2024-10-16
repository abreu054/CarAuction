using AutoFixture;
using CarAuction.Business.Dbo.Models.Auctions;
using CarAuction.Business.Validators;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarAuction.Application.Tests
{
    public class AuctionBidServiceTests
    {
        private readonly Fixture _fixture;

        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IDataRepository<Auction>> _auctionRepositoryMock;
        private readonly Mock<IDataRepository<AuctionBid>> _auctionBidRepositoryMock;

        private readonly AuctionBidService _service;

        public AuctionBidServiceTests()
        {
            _fixture = new Fixture();

            var userStore = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStore.Object, 
                null, 
                null, 
                null,
                null, 
                null,
                null,
                null,
                null);

            _auctionRepositoryMock = new Mock<IDataRepository<Auction>>();
            _auctionBidRepositoryMock = new Mock<IDataRepository<AuctionBid>>();

            _service = new AuctionBidService(
                Mock.Of<ILogger<AuctionBidService>>(),
                new AuctionBidValidator(),
                _userManagerMock.Object,
                _auctionRepositoryMock.Object,
                _auctionBidRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateAuctionBidAsync_ShouldReturnFalse_WhenAuctionID_IsZeroOrSmaller()
        {
            var auctionBidDto = _fixture
                .Build<CreateAuctionBidRequestDto>()
                .With(dto => dto.AuctionID, 0)
                .Create();

            var createEntityResponse = await _service.CreateAuctionBidAsync(auctionBidDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Auction ID cannot be empty");
        }

        [Fact]
        public async Task CreateAuctionBidAsync_ShouldReturnFalse_WhenAuctionStatus_IsInactive()
        {
            var auctionBidDto = _fixture
                .Build<CreateAuctionBidRequestDto>()
                .With(dto => dto.AuctionID, 1)
                .Create();

            _auctionRepositoryMock
                .Setup(repo => repo.GetByIDAsync(auctionBidDto.AuctionID))
                .ReturnsAsync(new Auction() { AuctionStatus = Business.Core.AuctionStatus.Inactive });

            var createEntityResponse = await _service.CreateAuctionBidAsync(auctionBidDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Auction must be active");
        }

        [Fact]
        public async Task CreateAuctionBidAsync_ShouldReturnFalse_WhenAuctionEndData_IsPast()
        {
            var auctionBidDto = _fixture
                .Build<CreateAuctionBidRequestDto>()
                .With(dto => dto.AuctionID, 1)                
                .Create();

            _auctionRepositoryMock
                .Setup(repo => repo.GetByIDAsync(auctionBidDto.AuctionID))
                .ReturnsAsync(new Auction() 
                { 
                    AuctionStatus = Business.Core.AuctionStatus.Active,
                    AuctionEndDate = DateTime.UtcNow.AddSeconds(-5)
                });

            var createEntityResponse = await _service.CreateAuctionBidAsync(auctionBidDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Auction must be active");
        }
    }
}