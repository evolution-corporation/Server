using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.TypeHandlers;

namespace Server.Entities;

public class User
{
    public string Id { get; set; }
    public string? NickName { get; set; }
    public string? Birthday { get; set; }
    public string? DisplayName { get; set; }
    public string? Status { get; set; }
    public Role Role { get; set; }
    public UserGender Gender { get; set; }
    public UserCategory Category { get; set; }
    public DateTime DateTimeRegistration { get; set; } = DateTime.Now;
    public bool HasPhoto { get; set; } 
    public IList<UserMeditation> UserMeditations { get; set; }
}