using DataAccess.Models;

namespace BusinessLogic.Models;

public class PotentialServiceScheduleLedgerEntry
{
    public ServiceDateRoleModel ServiceDateRole { get; set; }

    public ICollection<PersonWithUnavailabilityModel> PossiblePeople { get; set; } =
        new List<PersonWithUnavailabilityModel>();

    public int PossiblePeopleCount => PossiblePeople.Count();
}