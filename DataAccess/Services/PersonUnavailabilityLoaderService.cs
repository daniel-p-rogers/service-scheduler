using System.Globalization;
using CsvHelper;
using DataAccess.Extensions;
using DataAccess.Models;

namespace DataAccess;

public class PersonUnavailabilityLoaderService : IDataLoader<PersonUnavailabilityModel>
{
    public ICollection<PersonUnavailabilityModel> LoadData()
    {
        using var reader = new StreamReader("./CsvData/Unavailability.csv");
        using var csv = new CsvReader(reader, new CultureInfo("en-GB"));
        var records = csv.GetRecords<PersonUnavailabilityModel>().ToList();

        return records;
    }    
}