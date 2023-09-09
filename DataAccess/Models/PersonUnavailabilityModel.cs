namespace DataAccess.Models;

public record PersonUnavailabilityModel(string Name, DateOnly Date, ServiceTime? Time);