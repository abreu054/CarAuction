using AutoFixture;
using CarAuction.Business.Dbo.Models.Vehicles;
using CarAuction.Business.Validators;
using CarAuction.Structure.DataRepositories;
using CarAuction.Structure.Dto.Search;
using CarAuction.Structure.Dto.Write;
using CarAuction.Structure.Services.Vehicles;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarAuction.Application.Tests
{
    public class VehicleServiceTests
    {
        private readonly Fixture _fixture;

        private readonly Mock<IDataRepository<Vehicle>> _vehicleRepositoryMock;
        private readonly Mock<IDataRepository<VehicleModel>> _vehicleModelRepositoryMock;

        private readonly VehicleService _vehicleService;

        public VehicleServiceTests()
        {
            _fixture = new Fixture();

            _vehicleRepositoryMock = new Mock<IDataRepository<Vehicle>>();
            _vehicleModelRepositoryMock = new Mock<IDataRepository<VehicleModel>>();

            _vehicleService = new VehicleService(
                Mock.Of<ILogger<VehicleService>>(),
                new VehicleValidator(),
                _vehicleRepositoryMock.Object,
                _vehicleModelRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldReturnFalse_WhenVehicleIdentifier_IsEmpty()
        {
            var vehicleDto = _fixture.Build<CreateVehicleRequestDto>()
                .With(v => v.VehicleUniqueIdentifier, string.Empty)
                .Create();

            var createEntityResponse = await _vehicleService.CreateVehicleAsync(vehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Vehicle identifier cannot be empty");
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldReturnFalse_WhenVehicle_Exists()
        {
            var vehicleDto = _fixture.Create<CreateVehicleRequestDto>();

            _vehicleRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<VehiclesSearchParamsDto>()))
                .ReturnsAsync([new Vehicle()]);

            var createEntityResponse = await _vehicleService.CreateVehicleAsync(vehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("A vehicle with the same Identifier already exists");
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldReturnFalse_WhenModel_IsEqualOrLessThanZero()
        {
            var vehicleDto = _fixture.Create<CreateVehicleRequestDto>();
            vehicleDto.VehicleModelID = 0;

            _vehicleRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<VehiclesSearchParamsDto>()))
                .ReturnsAsync([]);

            var createEntityResponse = await _vehicleService.CreateVehicleAsync(vehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Vehicle model cannot be empty");
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldReturnFalse_WhenVehicleModel_IsNull()
        {
            var vehicleDto = _fixture.Create<CreateVehicleRequestDto>();

            _vehicleModelRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<VehiclesSearchParamsDto>()))
                .ReturnsAsync([]);

            var createEntityResponse = await _vehicleService.CreateVehicleAsync(vehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Vehicle model was not found");
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldReturnFalse_WhenValidator_HasErrors()
        {
            var createVehicleDto = _fixture.Create<CreateVehicleRequestDto>();
            createVehicleDto.VehicleYear = DateTime.UtcNow.AddYears(1).Year;

            _vehicleRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<VehiclesSearchParamsDto>()))
                .ReturnsAsync([]);

            _vehicleModelRepositoryMock
                .Setup(repo => repo.GetByIDAsync(createVehicleDto.VehicleModelID))
                .ReturnsAsync(new VehicleModel() { VehicleModelID = 1, VehicleManufacturerID = 1 });

            var createEntityResponse = await _vehicleService.CreateVehicleAsync(createVehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Year cannot be in the future");

            createVehicleDto.VehicleYear = DateTime.UtcNow.Year;
            createVehicleDto.VehicleStartingBid = 0;
            
            createEntityResponse = await _vehicleService.CreateVehicleAsync(createVehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Starting bid mandatory");

            createVehicleDto.VehicleStartingBid = 1;
            createVehicleDto.VehicleType = Business.Core.VehicleType.Hatchback;
            createVehicleDto.VehicleNumberOfDoors = 0;
            
            createEntityResponse = await _vehicleService.CreateVehicleAsync(createVehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Number of doors mandatory");

            createVehicleDto.VehicleType = Business.Core.VehicleType.SUV;
            createVehicleDto.VehicleNumberOfSeats = 0;

            createEntityResponse = await _vehicleService.CreateVehicleAsync(createVehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Number of seats mandatory");

            createVehicleDto.VehicleType = Business.Core.VehicleType.Truck;
            createVehicleDto.VehicleLoadCapacity = 0;

            createEntityResponse = await _vehicleService.CreateVehicleAsync(createVehicleDto);
            createEntityResponse.Success.Should().BeFalse();
            createEntityResponse.Message.Should().BeEquivalentTo("Load capacity mandatory");
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldReturnTrue_WhenAllCriterias_AreMet()
        {
            var createVehicleDto = _fixture.Create<CreateVehicleRequestDto>();
            createVehicleDto.VehicleYear = DateTime.UtcNow.Year;

            _vehicleRepositoryMock
                .Setup(repo => repo.SearchAsync(It.IsAny<VehiclesSearchParamsDto>()))
                .ReturnsAsync([]);

            _vehicleModelRepositoryMock
                .Setup(repo => repo.GetByIDAsync(createVehicleDto.VehicleModelID))
                .ReturnsAsync(new VehicleModel() { VehicleModelID = 1, VehicleManufacturerID = 1 });

            _vehicleRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Vehicle>()))
                .ReturnsAsync((true, ""));

            var createEntityResponse = await _vehicleService.CreateVehicleAsync(createVehicleDto);
            createEntityResponse.Success.Should().BeTrue();
            createEntityResponse.Message.Should().BeEquivalentTo("Vehicle created with success");
        }
    }
}