using BusinessLogic.Models;
using CsvHelper.Delegates;
using DataAccess;
using DataAccess.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class SchedulerService : BackgroundService
{
    private readonly IDataLoader<PersonModel> _personLoader;
    private readonly IDataLoader<ServiceDateRoleModel> _serviceDateRoleLoader;
    private readonly IDataLoader<PersonUnavailabilityModel> _personUnavailabilityLoader;
    private readonly ILogger<SchedulerService> _logger;

    public SchedulerService(
        IDataLoader<PersonModel> personLoader,
        IDataLoader<ServiceDateRoleModel> serviceDateRoleLoader,
        IDataLoader<PersonUnavailabilityModel> personUnavailabilityLoader,
        ILogger<SchedulerService> logger)
    {
        _personLoader = personLoader ?? throw new ArgumentNullException(nameof(personLoader));
        _serviceDateRoleLoader = serviceDateRoleLoader ?? throw new ArgumentNullException(nameof(serviceDateRoleLoader));
        _personUnavailabilityLoader = personUnavailabilityLoader ?? throw new ArgumentNullException(nameof(personUnavailabilityLoader));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var people = _personLoader.LoadData();
        var serviceDateRoles = _serviceDateRoleLoader.LoadData();
        var personUnavailability = _personUnavailabilityLoader.LoadData();

        var peopleWithUnavailability = people
            .Select(o => new
            {
                Person = o,
                Unavailability = personUnavailability.Where(pu => pu.Name == o.Name).ToList()
            })
            .Select(o => new PersonWithUnavailabilityModel(o.Person, o.Unavailability))
            .ToList();

        var serviceScheduleLedger = new List<ServiceScheduleLedgerEntry>();

        var potentialLedgerEntries = new List<PotentialServiceScheduleLedgerEntry>();

        foreach (var serviceDateRole in serviceDateRoles)
        {
            var peopleAvailable = peopleWithUnavailability
                .Where(p => p.IsAvailableForDate(serviceDateRole))
                .ToList();
            
            _logger.LogInformation("There are {count} people available for the {date} {serviceTime} service for {role}",
                peopleAvailable.Count().ToString(),
                serviceDateRole.Date.ToString(),
                serviceDateRole.ServiceRole.ServiceTime.ToString().ToString(),
                serviceDateRole.ServiceRole.Role.ToString());

            var potentialLedgerEntry = new PotentialServiceScheduleLedgerEntry()
            {
                ServiceDateRole = serviceDateRole,
                PossiblePeople = peopleAvailable
            };

            potentialLedgerEntries.Add(potentialLedgerEntry);
        }

        var iterationCount = 0;
        while (potentialLedgerEntries.Any())
        {
            iterationCount++;
            _logger.LogInformation("Iteration {iterationCount}: Beginning", iterationCount);
            _logger.LogInformation("Iteration {iterationCount}: {scheduledCount} have been scheduled, {remainingCount} still to go", iterationCount, serviceScheduleLedger.Count(), potentialLedgerEntries.Count());
            var potentialNextLedgerEntry = potentialLedgerEntries.OrderBy(o => o.PossiblePeopleCount).First();

            // Remove people who are already serving this week or a week either side.
            var peopleToExclude = potentialNextLedgerEntry.PossiblePeople
                .Where(p => serviceScheduleLedger
                    .Any(s => s.AssignedPerson == p &&
                              Math.Abs(s.ServiceDateRole.Date.DayNumber -
                                       potentialNextLedgerEntry.ServiceDateRole.Date.DayNumber) <= 7))
                .ToList();

            var newLedgerEntry = new ServiceScheduleLedgerEntry
            {
                ServiceDateRole = potentialNextLedgerEntry.ServiceDateRole,
                RejectedPeople = peopleToExclude,
                OtherPossiblePeople = potentialNextLedgerEntry.PossiblePeople.Except(peopleToExclude).ToList()
            };

            if (!newLedgerEntry.OtherPossiblePeople.Any())
            {
                // We need to rework the previous ledger entry as we can't find somebody to fill the role with current assignments
                var alternativeConfigurationFound = false;
                while (!alternativeConfigurationFound)
                {
                    if (!serviceScheduleLedger.Any())
                    {
                        throw new Exception("There is no way to complete this rota");
                    }
                    
                    var lastLedgerEntry = serviceScheduleLedger.Last();
                    lastLedgerEntry.RejectAssignedPerson();

                    if (lastLedgerEntry.OtherPossiblePeople.Any())
                    {
                        var bestCandidate = GetBestCandidate(lastLedgerEntry.OtherPossiblePeople, serviceScheduleLedger);
                        lastLedgerEntry.AssignPerson(bestCandidate);
                        alternativeConfigurationFound = true;
                    }
                    else
                    {
                        // We need to go back further and re-add this potential ledger entry to the pool
                        var restoredPotentialLedgerEntry = new PotentialServiceScheduleLedgerEntry
                        {
                            ServiceDateRole = lastLedgerEntry.ServiceDateRole,
                            PossiblePeople =
                                lastLedgerEntry.RejectedPeople // No possible people so all possibilities were rejected
                        };

                        potentialLedgerEntries.Add(restoredPotentialLedgerEntry);
                        serviceScheduleLedger.Remove(lastLedgerEntry);
                    }
                }
            }
            else
            {
                // Find the person who is currently both available and assigned least
                var bestCandidate = GetBestCandidate(newLedgerEntry.OtherPossiblePeople, serviceScheduleLedger);
                
                newLedgerEntry.AssignPerson(bestCandidate);
                serviceScheduleLedger.Add(newLedgerEntry);
                potentialLedgerEntries.Remove(potentialNextLedgerEntry);
            }
        }

        PrintSchedule(serviceScheduleLedger);

        return Task.CompletedTask;
    }

    private PersonWithUnavailabilityModel GetBestCandidate
        (ICollection<PersonWithUnavailabilityModel> peopleOptions,
            List<ServiceScheduleLedgerEntry> existingLedger)
    {
        var bestCandidate = peopleOptions
            .Select(o => new
            {
                Person = o,
                AssignedCount = existingLedger.Count(ssl => ssl.AssignedPerson == o)
            })
            .OrderBy(o => o.AssignedCount)
            .ThenBy(o => o.Person.PermittedSessions.Count) // Prioritise people who are limited in what they can cover
            .Select(o => o.Person)
            .First();

        return bestCandidate;
    }

    private void PrintSchedule(ICollection<ServiceScheduleLedgerEntry> serviceScheduleLedger)
    {
        var outputFileName = "schedule-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";

        using var writer = new StreamWriter(outputFileName);
        
        _logger.LogInformation("The schedule is as follows:");

        var orderedSchedule = serviceScheduleLedger
            .OrderBy(o => o.ServiceDateRole.Date)
            .ThenBy(o => o.ServiceDateRole.ServiceRole.ServiceTime)
            .ThenBy(o => o.ServiceDateRole.ServiceRole.Role)
            .ToList();

        var previousDate = orderedSchedule.Select(o => o.ServiceDateRole.Date).First();

        foreach (var ledgerItem in orderedSchedule)
        {
            var date = ledgerItem.ServiceDateRole.Date;
            var serviceTime = ledgerItem.ServiceDateRole.ServiceRole.ServiceTime;
            var role = ledgerItem.ServiceDateRole.ServiceRole.Role;
            var name = ledgerItem.AssignedPerson.Name;
            _logger.LogInformation("{date} {serviceTime} {role}: {name}",date, serviceTime, role, name);

            if (previousDate != date)
            {
                writer.WriteLine();
            }
            writer.WriteLine($"{date} {serviceTime} {role}: {name}");

            previousDate = date;
        }
        
        writer.WriteLine();
        writer.WriteLine("---");
        writer.WriteLine();
        
        _logger.LogInformation("Summary statistics:");
        
        var countsByPerson = orderedSchedule
            .GroupBy(o => o.AssignedPerson)
            .Select(o => new
            {
                PersonName = o.Key.Name,
                Count = o.Count()
            })
            .OrderByDescending(o => o.Count)
            .ToList();

        foreach (var person in countsByPerson)
        {
            _logger.LogInformation("{person} is on the rota {count} time(s)", person.PersonName, person.Count);
            writer.WriteLine($"{person.PersonName} is on the rota {person.Count} time(s)");
        }
    }
}