using System.ComponentModel.DataAnnotations;

namespace NexTrack.Models
{
    public class ChildItemInput
    {
        [Required(ErrorMessage = "Child item name is required.")]
        [StringLength(200)]
        [Display(Name = "Item Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Weight is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
        [Display(Name = "Weight (kg)")]
        public decimal Weight { get; set; }
    }

    public class ProcessItemViewModel
    {
        [Required(ErrorMessage = "Please select a parent item to process.")]
        [Display(Name = "Select Parent Item")]
        public int ParentId { get; set; }

        public List<ChildItemInput> Children { get; set; } = new List<ChildItemInput>
        {
            new ChildItemInput()
        };

        // For populating the dropdown
        public List<Item> PendingItems { get; set; } = new List<Item>();
    }
}
