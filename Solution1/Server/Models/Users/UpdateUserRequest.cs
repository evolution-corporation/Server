namespace Server.Models.Users;

using Entities;

public class UpdateUserRequest
{
    public string? NickName { get; set; }
    public Role Role { get; set; }
    
    public DateTime Birthday { get; set; }
    
    public string? Status { get; set; }
    
    public UserGender Gender { get; set; }
    
    public UserCategory Category { get; set; }
    
    public string? Image { get; set; }
    
    public string? DisplayName { get; set; }
    public string? ExpoToken { get; set; }
}