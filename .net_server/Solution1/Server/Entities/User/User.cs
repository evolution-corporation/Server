using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.TypeHandlers;

namespace WebApi.Entities;

public class User
{
    
    public Guid Id { get; set; }
    public string NickName { get; set; }
    public string Birthday { get; set; }
    public Guid ImageId { get; set; }
    public string DisplayName { get; set; }
    public string Status { get; set; }
    public Role Role { get; set; }
    public UserGender Gender { get; set; }
    public UserCategory Category { get; set; }
    public DateTime DateTimeRegistration { get; set; }
    public List<int> ListenedMeditation { get; set; }
}