using DataAccess.Models;

namespace DataAccess.StaticData;

public static class SessionData
{
    public static ServiceRoleModel MorningSound => new ServiceRoleModel(Role.Sound, ServiceTime.Morning);
    public static ServiceRoleModel MorningVisuals => new ServiceRoleModel(Role.Visuals, ServiceTime.Morning);
    public static ServiceRoleModel MorningStreaming => new ServiceRoleModel(Role.Streaming, ServiceTime.Morning);
    public static ServiceRoleModel EveningSound => new ServiceRoleModel(Role.Sound, ServiceTime.Evening);
    public static ServiceRoleModel EveningVisuals => new ServiceRoleModel(Role.Visuals, ServiceTime.Evening);
}