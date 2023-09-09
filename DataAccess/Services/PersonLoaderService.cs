using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using CsvHelper;
using DataAccess.Extensions;
using DataAccess.Models;

namespace DataAccess;

public class PersonLoaderService : IDataLoader<PersonModel>
{
    public ICollection<PersonModel> LoadData()
    {
        using var reader = new StreamReader("./CsvData/People.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<PersonCsvModel>().ToList();

        var formattedPeopleRecords = records
            .Select(r => new PersonModel(r.Name, r.GetSessionsForPerson()))
            .ToList();

        return formattedPeopleRecords;
    }
}