using DataAccess.Models;
using DataAccess.StaticData;

namespace DataAccess.Extensions;

public static class SessionDateExtensions
{
    public static ICollection<ServiceDateRoleModel> GetRequiredServiceDateRoles(this ServiceDateCsvModel serviceDateCsvModel)
    {
        var serviceDateRoleModels = new List<ServiceDateRoleModel>();

        if (serviceDateCsvModel.MorningService)
        {
            serviceDateRoleModels.Add(new ServiceDateRoleModel(serviceDateCsvModel.Date, SessionData.MorningSound));
            serviceDateRoleModels.Add(new ServiceDateRoleModel(serviceDateCsvModel.Date, SessionData.MorningVisuals));
            serviceDateRoleModels.Add(new ServiceDateRoleModel(serviceDateCsvModel.Date, SessionData.MorningStreaming));
        }

        if (serviceDateCsvModel.EveningService)
        {
            serviceDateRoleModels.Add(new ServiceDateRoleModel(serviceDateCsvModel.Date, SessionData.EveningSound));
            serviceDateRoleModels.Add(new ServiceDateRoleModel(serviceDateCsvModel.Date, SessionData.EveningVisuals));
        }
        
        return serviceDateRoleModels;
    }
}