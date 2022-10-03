namespace Server.Models.Users;

using System.ComponentModel.DataAnnotations;
using Entities;

//Request to create user
public class CreateUserRequest
{
#pragma warning disable CS8618
    [Required] public string NickName { get; set; }
#pragma warning restore CS8618

    [Required] public DateTime Birthday { get; set; }
    public string? ExpoToken { get; set; }
    public string? Status { get; set; }

    public UserGender Gender { get; set; }

    public UserCategory Category { get; set; }
    public string? Image { get; set; }
    public string? DisplayName { get; set; }
}