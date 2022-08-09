using System.ComponentModel.DataAnnotations;

namespace Server.Entities;

public class Subscribe
{
    [Key]
    public string UserId { get; set; }
    public DateTime WhenSubscribe { get; set; }
    public int TimeSubscribe { get; set; }
    
}