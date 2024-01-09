using CreateUser.Core.Repository;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CreateUser.Infrastructure;
public class ConMysqlConnection : IDataBaseConnection
{
    private string connectionString;
    private readonly IConfiguration _configuration;

    public ConMysqlConnection(IConfiguration configuration)
    {
        _configuration = configuration;
        var host = _configuration["MysqlConfig:Host"]!;
        var userName = _configuration["MysqlConfig:UserName"]!;
        var password = _configuration["MysqlConfig:Password"]!;
        var port = _configuration["MysqlConfig:Port"]!;

        connectionString = $"server={host};port={port};database=TesteUser;user={userName};password={password}";
    }

    public void ExecuteInsert(string query, object parameters)
    {
        using var connection = new MySqlConnection(connectionString);
        var result = connection.Execute(query, parameters);
    }
}