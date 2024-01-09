using CreateUser.Core.Entities;
using CreateUser.Core.ValueObjects;

namespace CreateUser.API.Model.Request;
public class UserRequest
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Document { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public static implicit operator User(UserRequest request) =>
        new User(request.Name,
            new Email(request.Email),
            new Document(request.Document),
            new Phone(request.Phone)
            );
}