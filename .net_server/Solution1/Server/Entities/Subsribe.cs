using System.ComponentModel.DataAnnotations;

namespace Server.Entities;

public class Subscribe
{
    [Key]
    public Guid UserId { get; set; }
    public DateTime WhenSubscribe { get; set; }
    public DateTime TimeSubscribe { get; set; }
}