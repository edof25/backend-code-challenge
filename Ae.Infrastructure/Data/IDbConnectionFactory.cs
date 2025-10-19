using System.Data;

namespace Ae.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
