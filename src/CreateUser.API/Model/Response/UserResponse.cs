using CreateUser.Core.Entities;

namespace CreateUser.API.Model.Response;

public class UserResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Document { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public static implicit operator UserResponse(User user) =>
        new UserResponse()
        {
            Name = user.Name,
            Email = user.Email.Address,
            Document = user.Document.Number,
            Phone = user.Phone.Number
        };
}