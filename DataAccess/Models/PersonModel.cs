namespace DataAccess.Models;

public record PersonModel(
    string Name,
    int? MaximumServicesInPeriod,
    int IdealDaysBetweenServices,
    ICollection<ServiceRoleModel> PermittedSessions);