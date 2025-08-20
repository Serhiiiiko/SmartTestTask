using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartTestTask.Application.DTOs.Request;
using SmartTestTask.Application.DTOs.Responce;
using SmartTestTask.Infrastructure.Data;

namespace SmartTestTask.Tests.Controllers;

public class ContractsControllerTests : IClassFixture<WebApplicationFactory<SmartTestTask.API.Program>>
{
    private readonly WebApplicationFactory<SmartTestTask.API.Program _factory;
    private const string ApiKey = "TEST_API_KEY_12345";

    public ContractsControllerTests(WebApplicationFactory<SmartTestTask.API.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add InMemory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                // Override configuration
                services.Configure<Microsoft.Extensions.Configuration.IConfiguration>(config =>
                {
                    config["ApiKey"] = ApiKey;
                });
            });
        });
    }

    [Fact]
    public async Task GetAllContracts_WithValidApiKey_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

        // Act
        var response = await client.GetAsync("/api/contracts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllContracts_WithoutApiKey_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/contracts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateContract_WithValidData_ReturnsCreatedStatus()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

        var createDto = new CreateContractDto
        {
            ProductionFacilityCode = "FAC-001",
            ProcessEquipmentTypeCode = "EQT-001",
            EquipmentQuantity = 5
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/contracts", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.EquipmentQuantity.Should().Be(5);
    }

    [Fact]
    public async Task CreateContract_WithInvalidQuantity_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

        var createDto = new CreateContractDto
        {
            ProductionFacilityCode = "FAC-001",
            ProcessEquipmentTypeCode = "EQT-001",
            EquipmentQuantity = 0 // Invalid quantity
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/contracts", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetContractById_WithExistingId_ReturnsContract()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

        // First create a contract
        var createDto = new CreateContractDto
        {
            ProductionFacilityCode = "FAC-001",
            ProcessEquipmentTypeCode = "EQT-001",
            EquipmentQuantity = 3
        };

        var createResponse = await client.PostAsJsonAsync("/api/contracts", createDto);
        var createdContract = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        var contractId = createdContract.Data.Id;

        // Act
        var response = await client.GetAsync($"/api/contracts/{contractId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Id.Should().Be(contractId);
    }

    [Fact]
    public async Task GetContractById_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/contracts/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateContract_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

        // First create a contract
        var createDto = new CreateContractDto
        {
            ProductionFacilityCode = "FAC-001",
            ProcessEquipmentTypeCode = "EQT-001",
            EquipmentQuantity = 3
        };

        var createResponse = await client.PostAsJsonAsync("/api/contracts", createDto);
        var createdContract = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        var contractId = createdContract.Data.Id;

        // Update the contract
        var updateDto = new UpdateContractDto
        {
            Id = contractId,
            EquipmentQuantity = 5
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/contracts/{contractId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.EquipmentQuantity.Should().Be(5);
    }

    [Fact]
    public async Task DeactivateContract_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

        // First create a contract
        var createDto = new CreateContractDto
        {
            ProductionFacilityCode = "FAC-001",
            ProcessEquipmentTypeCode = "EQT-001",
            EquipmentQuantity = 3
        };

        var createResponse = await client.PostAsJsonAsync("/api/contracts", createDto);
        var createdContract = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ContractDto>>();
        var contractId = createdContract.Data.Id;

        // Act
        var response = await client.DeleteAsync($"/api/contracts/{contractId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }
}