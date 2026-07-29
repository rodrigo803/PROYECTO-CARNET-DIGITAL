using System.Data;

namespace AccessControlService.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}