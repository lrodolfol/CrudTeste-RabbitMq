using CreateUser.Core.Repository;
using Dapper;
using MySql.Data.MySqlClient;

namespace CreateUser.Infrastructure;
public class ConMysqlConnection : IDataBaseConnection
{
    private string connectionString = "server=localhost;port=3306;database=TesteUser;user=root;password=teste123";

    public void ExecuteInsert(string query, object parameters)
    {
        using var connection = new MySqlConnection(connectionString);
        var result = connection.Execute(query, parameters);
    }
}