using DataAccess.Models;

namespace BusinessLogic.Models;

public class ServiceScheduleLedgerEntry
{
    public ServiceDateRoleModel ServiceDateRole { get; set; }
    public PersonWithUnavailabilityModel AssignedPerson { get; set; }
    public ICollection<PersonWithUnavailabilityModel> OtherPossiblePeople { get; set; }
    public ICollection<PersonWithUnavailabilityModel> RejectedPeople { get; set; }

    public void RejectAssignedPerson()
    {
        RejectedPeople.Add(AssignedPerson);
        AssignedPerson = null;
    }

    public void AssignPerson(PersonWithUnavailabilityModel personToAssign)
    {
        OtherPossiblePeople.Remove(personToAssign);
        AssignedPerson = personToAssign;
    }
}