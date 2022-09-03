using System.ComponentModel.DataAnnotations;
using Server.Entities;

namespace Server.Models.Meditation;

public class MeditationPreferences
{
    public TypeMeditation? TypeMeditation { get; set; }
    public CountDayMeditation? CountDay { get; set; }
    public TimeMeditation? Time { get; set; }
}