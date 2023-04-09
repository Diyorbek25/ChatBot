using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBot.Core.Models;


public class User
{
    public long Id { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }

    public ICollection<SelectedUser> SelectedUsers { get; set; } = new List<SelectedUser>();
    public long? AddressId { get; set; }
    public Address? Address { get; set; }
    public Role Role { get; set; } = Role.User;
}

public enum Role
{
    Admin,
    User
}
