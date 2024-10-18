using AutoFixture;
using CarAuction.Business.Dbo.Models.Auctions;
using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Business.Validators;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Search;
using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarAuction.Application.Tests
{
    public class AuctionServiceTests
    {
        private readonly Fixture _fixture;

        private readonly Mock<IDataRepository<Auction>> _auctionRepositoryMock;
        private readonly Mock<IDataRepository<Vehicle>> _vehicleRepositoryMock;

        private readonly AuctionService _service;

        public AuctionServiceTests()
        {
            _fixture = new Fixture();

            _auctionRepositoryMock = new Mock<IDataRepository<Auction>>();
            _vehicleRepositoryMock = new Mock<IDataRepository<Vehicle>>();

            _service = new AuctionService(
                Mock.Of<ILogger<AuctionService>>(),
                new AuctionValidator(),
                _auctionRepositoryMock.Object,
                _vehicleRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateAuctionAsync_ShouldReturnFalse_WhenAuctionVehicleID_IsEqualOrLessThanZero()
        {
            var auctionDto = _fixture
                .Build<CreateAuctionRequestDto>()
                .With(dto => dto.VehicleID, 0)
                .Create();

            var createEntityResponse = await _service.CreateAuctionAsync(auctionDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Vehicle mandatory");
        }

        [Fact]
        public async Task CreateAuctionAsync_ShouldReturnFalse_WhenAuctionVehicle_IsNull()
        {
            var auctionDto = _fixture.Create<CreateAuctionRequestDto>();

            var createEntityResponse = await _service.CreateAuctionAsync(auctionDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo($"No vehicle found with ID {auctionDto.VehicleID}");
        }

        [Fact]
        public async Task CreateAuctionAsync_ShouldReturnFalse_WhenOtherAuction_IsActive()
        {
            var auctionDto = _fixture
                .Build<CreateAuctionRequestDto>()
                .With(dto => dto.AuctionStatus, Business.Core.AuctionStatus.Active)
                .Create();

            _vehicleRepositoryMock
                .Setup(repo => repo.GetByIDAsync(auctionDto.VehicleID))
                .ReturnsAsync(new Vehicle());

            _auctionRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<AuctionSearchParamsDto>()))
                .ReturnsAsync([new Auction()]);

            var createEntityResponse = await _service.CreateAuctionAsync(auctionDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("There is an active auction for this vehicle");
        }

        [Fact]
        public async Task CreateAuctionAsync_ShouldReturnFalse_WhenStartDate_GreaterThanEndDate()
        {
            var auctionDto = _fixture
                .Build<CreateAuctionRequestDto>()
                .With(dto => dto.AuctionStartDate, DateTime.UtcNow)
                .With(dto => dto.AuctionEndDate, DateTime.UtcNow.AddYears(-1))
                .Create();

            _vehicleRepositoryMock
                .Setup(repo => repo.GetByIDAsync(auctionDto.VehicleID))
                .ReturnsAsync(new Vehicle() { VehicleID = 1, VehicleUniqueIdentifier = "Test_1" });

            _auctionRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<AuctionSearchParamsDto>()))
                .ReturnsAsync([]);

            var createEntityResponse = await _service.CreateAuctionAsync(auctionDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Auction end date must be bigger than start date");
        }

        [Fact]
        public async Task CreateAuctionAsync_ShouldReturnFalse_WhenStatus_Active_AndDates_NotValid()
        {
            var auctionDto = _fixture
                .Build<CreateAuctionRequestDto>()
                .With(dto => dto.AuctionStatus, Business.Core.AuctionStatus.Active)
                .With(dto => dto.AuctionStartDate, DateTime.UtcNow.AddDays(1))
                .With(dto => dto.AuctionEndDate, DateTime.UtcNow.AddDays(10))
                .Create();

            _vehicleRepositoryMock
                .Setup(repo => repo.GetByIDAsync(auctionDto.VehicleID))
                .ReturnsAsync(new Vehicle() { VehicleID = 1, VehicleUniqueIdentifier = "Test_1" });

            _auctionRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<AuctionSearchParamsDto>()))
                .ReturnsAsync([]);

            var createEntityResponse = await _service.CreateAuctionAsync(auctionDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Auction cannot be active when start and end dates are not valid");
        }

        [Fact]
        public async Task CreateAuctionAsync_ShouldReturnTrue_WhenAllCriterias_AreMet()
        {
            var auctionDto = _fixture
                .Build<CreateAuctionRequestDto>()
                .With(dto => dto.AuctionStartDate, DateTime.UtcNow)
                .With(dto => dto.AuctionEndDate, DateTime.UtcNow.AddYears(1))
                .Create();

            _vehicleRepositoryMock
                .Setup(repo => repo.GetByIDAsync(auctionDto.VehicleID))
                .ReturnsAsync(new Vehicle() { VehicleID = 1, VehicleUniqueIdentifier = "Test_1" });

            _auctionRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<AuctionSearchParamsDto>()))
                .ReturnsAsync([]);

            _auctionRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Auction>()))
                .ReturnsAsync((true, ""));

            var createEntityResponse = await _service.CreateAuctionAsync(auctionDto);
            createEntityResponse.Success.Should().BeTrue();
            createEntityResponse.Message.Should().BeEquivalentTo("Auction created with success");
        }
    }
}