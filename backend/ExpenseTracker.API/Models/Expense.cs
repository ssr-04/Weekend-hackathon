using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.API.Models
{
    public class Expense : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}