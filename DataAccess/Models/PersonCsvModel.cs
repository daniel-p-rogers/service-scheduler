namespace DataAccess.Models;

public record PersonCsvModel(
    string Name,
    int? MaximumServicesInPeriod,
    int IdealDaysBetweenServices,
    bool MorningSound,
    bool MorningVisuals,
    bool MorningStreaming,
    bool EveningSound,
    bool EveningVisuals);