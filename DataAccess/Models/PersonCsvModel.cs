namespace DataAccess.Models;

public record PersonCsvModel(
    string Name,
    bool MorningSound,
    bool MorningVisuals,
    bool MorningStreaming,
    bool EveningSound,
    bool EveningVisuals);