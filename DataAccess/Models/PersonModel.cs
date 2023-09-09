namespace DataAccess.Models;

public record PersonModel(string Name, ICollection<ServiceRoleModel> PermittedSessions);