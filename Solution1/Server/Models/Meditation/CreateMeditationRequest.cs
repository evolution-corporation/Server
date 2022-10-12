// using System.ComponentModel.DataAnnotations;
// using Server.Entities;
//
// namespace Server.Models.Meditation;
//
// public class CreateMeditationRequest
// {
//     [Required]
//     public Guid Id { get; set; }
//     [Required]
// #pragma warning disable CS8618
//     public Dictionary<string,string> Name { get; set; }
//     [Required]
//     public Dictionary<string,string> Description { get; set; }
// #pragma warning restore CS8618
//     [Required]
//     public TypeMeditation TypeMeditation { get; set; }
//     [Required]
//     public TimeMeditation TimeMeditation { get; set; }
//     [Required]
//     public CountDayMeditation CountDayMeditation { get; set; }
//     [Required]
//     public Dictionary<string,string>? Language { get; set; }
//     [Required]
//     public int AudioLength { get; set; }
//     [Required]
//     public Dictionary<string,Guid>? AudioIds { get; set; }
//     public Entities.Subscription? Subscription { get; set; }
//     public string? SubscriptionPhoto { get; set; }   
//     public string? MeditationPhoto { get; set; }
//     
//     public bool IsSubscribed { get; set; }
// }