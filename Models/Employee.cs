
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApi.Models;

[Index(nameof(Email), IsUnique = true)]
public class Employee
{
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999.99")]
    [Precision(18, 2)]
    public decimal Salary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
