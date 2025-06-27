namespace ExpenseTracker.API.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid? UserId { get; set; } // Null for predefined categories
        public User? User { get; set; }
        public bool IsPredefined { get; set; } = false;

        // Navigation Properties
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}