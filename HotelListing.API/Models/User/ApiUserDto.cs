using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.User;

public class ApiUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 6, ErrorMessage = "Your Password is limited to {2} to {1} characters")]
    public string Password { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}
