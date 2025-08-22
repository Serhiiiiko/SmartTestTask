namespace SmartTestTask.Application.DTOs.Responce;

public record ProductionFacilityDto(
    string Code,
    string Name,
    decimal StandardArea,
    decimal OccupiedArea,
    decimal AvailableArea);