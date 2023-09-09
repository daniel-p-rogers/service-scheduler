using DataAccess.Models;
using DataAccess.StaticData;

namespace DataAccess.Extensions;

public static class PersonExtensions
{
    public static ICollection<ServiceRoleModel> GetSessionsForPerson(this PersonCsvModel personCsvModel)
    {
        var sessionModels = new List<ServiceRoleModel>();
            
        if (personCsvModel.MorningSound) { sessionModels.Add(SessionData.MorningSound); }
        if (personCsvModel.MorningVisuals) { sessionModels.Add(SessionData.MorningVisuals); }
        if (personCsvModel.MorningStreaming) { sessionModels.Add(SessionData.MorningStreaming); }
        if (personCsvModel.EveningSound) { sessionModels.Add(SessionData.EveningSound); }
        if (personCsvModel.EveningVisuals) { sessionModels.Add(SessionData.EveningVisuals); }

        return sessionModels;
    }
}