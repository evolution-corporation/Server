namespace Server.Models.Users;

using System.ComponentModel.DataAnnotations;
using Entities;

//Request to create user
public class CreateUserRequest
{
    [Required] public string NickName { get; set; }

    public DateTime Birthday { get; set; }

    public string Status { get; set; }

    public UserGender Gender { get; set; }

    public UserCategory Category { get; set; }

    public string Image { get; set; }

    public string DisplayName { get; set; }
    
    public string ExpoToken { get; set; }
}