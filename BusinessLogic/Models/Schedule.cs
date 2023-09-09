using DataAccess.Models;

namespace BusinessLogic.Models;

public class Schedule
{
    public ICollection<PersonWithUnavailabilityModel> People { get; set; }
    public ICollection<ServiceDateRoleModel> SessionDates { get; set; }
}