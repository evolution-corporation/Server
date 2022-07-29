namespace WebApi.Models.Users;

using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

public class UpdateUserRequest
{
    public string NickName { get; set; }
    
    public Role Role { get; set; }
    
    public DateTime Birthday { get; set; }
    
    public string Status { get; set; }
    
    public UserGender Gender { get; set; }
    
    public UserCategory Category { get; set; }
    
    public string Image { get; set; }
    
    public string DisplayName { get; set; }
    //
    // public string Title { get; set; }
    // public string FirstName { get; set; }
    // public string LastName { get; set; }
    //
    // [EnumDataType(typeof(Role))]
    // public string Role { get; set; }
    //
    // [EmailAddress]
    // public string Email { get; set; }
    //
    // // treat empty string as null for password fields to 
    // // make them optional in front end apps
    // private string _password;
    // [MinLength(6)]
    // public string Password
    // {
    //     get => _password;
    //     set => _password = replaceEmptyWithNull(value);
    // }
    //
    // private string _confirmPassword;
    // [Compare("Password")]
    // public string ConfirmPassword 
    // {
    //     get => _confirmPassword;
    //     set => _confirmPassword = replaceEmptyWithNull(value);
    // }
    //
    // // helpers
    //
    // private string replaceEmptyWithNull(string value)
    // {
    //     // replace empty string with null to make field optional
    //     return string.IsNullOrEmpty(value) ? null : value;
    // }
}