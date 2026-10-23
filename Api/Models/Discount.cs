using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class Discount
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 100)]
    public decimal Percentage { get; set; }
}
