using System.ComponentModel.DataAnnotations;

namespace TestTask.UserApi.ApiModels;

public class CreateUserRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}