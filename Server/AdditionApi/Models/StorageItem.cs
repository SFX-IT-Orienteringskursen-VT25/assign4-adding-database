using System.ComponentModel.DataAnnotations;

namespace AdditionApi.Models
{
    public class StorageItem
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Key { get; set; } = string.Empty;
        
        [Required]
        public string Value { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}