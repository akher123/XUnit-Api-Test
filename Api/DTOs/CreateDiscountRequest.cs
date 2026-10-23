using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class CreateDiscountRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 100)]
    public decimal Percentage { get; set; }
}
