using CreateUser.Core.Entities;
using CreateUser.Core.Repository;
using CreateUser.Repository;

namespace CreateUser.Infrastructure;
public class UserPostRepository : IUserPostRepository
{
    private readonly IDataBaseConnection _connection;

    public UserPostRepository(IDataBaseConnection connection) =>
        (_connection) = (connection);

    public void Execute(User user)
    {
        var query = "INSERT INTO user(name, email, document, phone, password) VALUES (@name, @email, @document, @phone, @password)";
        var parameters = new
        {
            name = user.Name,
            email = user.Email.Address,
            document = user.Document.Number,
            phone = user.Phone.Number,
            password = user.Password
        };

        _connection.ExecuteInsert(query, parameters);
    }
}
