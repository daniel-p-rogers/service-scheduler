using System.Globalization;
using CsvHelper;
using DataAccess.Extensions;
using DataAccess.Models;

namespace DataAccess;

public class ServiceDateRoleLoaderService : IDataLoader<ServiceDateRoleModel>
{
    public ICollection<ServiceDateRoleModel> LoadData()
    {
        var exeFolder = AppContext.BaseDirectory;
        var dataPath = Path.Combine(exeFolder, "ServiceDates.csv");
        using var reader = new StreamReader(dataPath);
        using var csv = new CsvReader(reader, new CultureInfo("en-GB"));
        var records = csv.GetRecords<ServiceDateCsvModel>().ToList();

        var formattedSessionDateRecords = records
            .SelectMany(r => r.GetRequiredServiceDateRoles())
            .ToList();

        return formattedSessionDateRecords;
    }    
}