using System;
using System.ComponentModel.DataAnnotations;
using TestTask.UserApi.Attributes;

namespace TestTask.UserApi.ApiModels;

public class CreateSubscriptionRequest
{
    [Required]
    [OneOf("Free", "Trial", "Super")]
    public string Type { get; set; }
    
    [Required]
    public DateTime? StartDate { get; set; }
    
    [Required]
    public DateTime? EndDate { get; set; }
}