namespace CreateUser.Core.Repository;

public interface IDataBaseConnection
{
    void ExecuteInsert(string query, object parameters);
}