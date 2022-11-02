namespace Server.Models.Users;

using Entities;

public class UpdateUserRequest
{
    public string? NickName { get; set; }

    public DateTime? Birthday { get; set; }
    public UserGender Gender { get; set; }
    public string? Image { get; set; }
    
    public string? DisplayName { get; set; }
    
}