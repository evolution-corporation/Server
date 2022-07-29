namespace WebApi.Models.Users;

using System.ComponentModel.DataAnnotations;
using WebApi.Entities;


//Request to create user
public class CreateUserRequest
{
    [Required]
    public string NickName { get; set; }
    
    public DateTime Birthday { get; set; }
    
    public string Status { get; set; }
    
    public UserGender Gender { get; set; }
    
    public UserCategory Category { get; set; }

    public string Image { get; set; }
    
    public string DisplayName { get; set; }
    
    // [Required]
    // public string Title { get; set; }
    //
    // [Required]
    // public string FirstName { get; set; }
    //
    // [Required]
    // public string LastName { get; set; }
    //
    // [Required]
    // [EnumDataType(typeof(Role))]
    // public string Role { get; set; }
    //
    // [Required]
    // [EmailAddress]
    // public string Email { get; set; }
    //
    // [Required]
    // [MinLength(6)]
    // public string Password { get; set; }
    //
    // [Required]
    // [Compare("Password")]
    // public string ConfirmPassword { get; set; }
}