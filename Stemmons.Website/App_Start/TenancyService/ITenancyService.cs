using System.Collections.Generic;

namespace Stemmons.TenancyUtilities
{
    public interface ITenancyService
    {
        string GetCurrentTenantDBConnection(TenantDatabaseType databaseType);
        bool IsTenantSelected { get; }
        List<TenantInfo> GetUserTenants();
    }

    public enum TenantDatabaseType
    {
        Cases,
        Entities,
        Quest = 2,
        Standards = 3,
        Central = 4,
        Mobile = 5,
        Cast = 6,
        CLG = 7,
        Facts = 8,
        Departments = 9
    }
}
