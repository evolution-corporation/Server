using System.ComponentModel.DataAnnotations;
using Server.Entities;

namespace Server.Models.Meditation;

public class MeditationPreferences
{
    [Required]
    public TypeMeditation[] TypeMeditation { get; set; }
    [Required]
    public CountDayMeditation CountDay { get; set; }
    [Required]
    public TimeMeditation Time { get; set; }
}