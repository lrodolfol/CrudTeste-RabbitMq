using CreateUser.Core.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreateUser.Core.Entities;

public class User : Entitie
{
    public User(string name, Email email, Document document, Phone phone) =>
        (Name, Email, Document, Phone) = (name, email, document, phone);

    public string Name { get; }
    [Column("Email")]
    public Email Email { get; }
    [Column("Document")]
    public Document Document { get; }
    [Column("Phone")]
    public Phone Phone { get; }
    [Column("Password")]
    public string? Password { get; private set; }

    public void SetPassword(string password) 
        => Password = password;

    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Document.Number) || string.IsNullOrEmpty(Phone.Number) || string.IsNullOrEmpty(Name))
            return false;

        return true;
    }


    public override string ToString()
    {
        var msg = $@"
                    Name: {Name}
                    Email {Email.Address}
                    Document: {Document.Number}
                    Phone: {Phone.Number}";

        return msg;
    }
}