using DataAccess.Models;

namespace BusinessLogic.Models;

public class PersonWithUnavailabilityModel
{
    public PersonWithUnavailabilityModel(PersonModel personModel, ICollection<PersonUnavailabilityModel> unavailabilityModels)
    {
        Name = personModel.Name;
        PermittedSessions = personModel.PermittedSessions;
        UnavailableSessions = unavailabilityModels
            .Where(o => o.Name == Name)
            .Select(o => new Unavailability(o.Date, o.Time))
            .ToList();

    }
    public string Name { get; set; }
    public ICollection<ServiceRoleModel> PermittedSessions { get; set; }
    public ICollection<Unavailability> UnavailableSessions { get; set; }

    public bool IsAvailableForDate(ServiceDateRoleModel serviceDateRole)
    {
        var role = serviceDateRole.ServiceRole.Role;
        var time = serviceDateRole.ServiceRole.ServiceTime;
        var date = serviceDateRole.Date;
        
        var session = new ServiceRoleModel(role, time);

        if (!PermittedSessions.Contains(session))
        {
            return false;
        }

        var unavailability = new Unavailability(date, time);
        var timelessUnavailability = new Unavailability(date, null);

        return !UnavailableSessions.Contains(unavailability) && !UnavailableSessions.Contains(timelessUnavailability);
    }
}