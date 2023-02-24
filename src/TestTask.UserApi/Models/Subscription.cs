using System;

namespace TestTask.UserApi.Models;

public class Subscription
{
    public int Id { get; set; }
    public string Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}