using System.ComponentModel.DataAnnotations;
using Server.Entities;
using Server.Entities.Mediatation;

namespace Server.Models.Meditation;

public class MeditationPreferences
{
    public TypeMeditation? TypeMeditation { get; set; }
    public TimeMeditation? Time { get; set; }
}