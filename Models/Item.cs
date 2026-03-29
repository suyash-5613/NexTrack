using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexTrack.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        [Display(Name = "Item Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Weight is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        [Display(Name = "Weight (kg)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }

        [Display(Name = "Parent Item")]
        public int? ParentId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "pending";

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ParentId")]
        public virtual Item? Parent { get; set; }

        public virtual ICollection<Item> Children { get; set; } = new List<Item>();
    }
}
