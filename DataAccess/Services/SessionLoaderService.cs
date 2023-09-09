using DataAccess.Models;
using DataAccess.StaticData;

namespace DataAccess;

public class SessionLoaderService : IDataLoader<ServiceRoleModel>
{
    public ICollection<ServiceRoleModel> LoadData()
    {
        return new List<ServiceRoleModel>()
        {
            SessionData.MorningSound,
            SessionData.MorningVisuals,
            SessionData.MorningStreaming,
            SessionData.EveningSound,
            SessionData.EveningVisuals
        };
    }
}