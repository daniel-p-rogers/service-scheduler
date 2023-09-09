namespace DataAccess.Models;

public record ServiceDateCsvModel(DateOnly Date, bool MorningService, bool EveningService);