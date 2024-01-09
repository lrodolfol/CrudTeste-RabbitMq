using CreateUser.Core.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;
using WorkerCreateUserConsumer.ValueObjects;

namespace WorkerCreateUserConsumer.Entities;

public class User : Entitie
{
    public User(string name, Email email, Document document, Phone phone, string password) =>
        (Name, Email, Document, Phone, Password) = (name, email, document, phone, password);

    public string Name { get; }
    public Email Email { get; }
    public Document Document { get; }
    public Phone Phone { get; }
    public string Password { get; }

    public bool IsValid() =>
        !(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email.Address) ||
        string.IsNullOrEmpty(Document.Number) || string.IsNullOrEmpty(Phone.Number));
}
