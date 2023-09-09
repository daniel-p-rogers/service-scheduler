using System.Globalization;
using CsvHelper;
using DataAccess.Extensions;
using DataAccess.Models;

namespace DataAccess;

public class ServiceDateRoleLoaderService : IDataLoader<ServiceDateRoleModel>
{
    public ICollection<ServiceDateRoleModel> LoadData()
    {
        using var reader = new StreamReader("./CsvData/ServiceDates.csv");
        using var csv = new CsvReader(reader, new CultureInfo("en-GB"));
        var records = csv.GetRecords<ServiceDateCsvModel>().ToList();

        var formattedSessionDateRecords = records
            .SelectMany(r => r.GetRequiredServiceDateRoles())
            .ToList();

        return formattedSessionDateRecords;
    }    
}