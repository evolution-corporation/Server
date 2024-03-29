﻿using System.ComponentModel.DataAnnotations;

namespace Server.Entities;

public class Subscription
{
    [Key]
    public int MeditationId { get; set; }
    public string Headers { get; set; }
    public string Description { get; set; }
    public string PayloadText { get; set; }
}