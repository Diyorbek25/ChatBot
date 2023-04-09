using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBot.Core.Models;

public class SelectedUser
{
    [Key]
    public int Id { get; set; }

    public long UserId { get; set; }
    public long? SelectedUserId { get; set; }
}
