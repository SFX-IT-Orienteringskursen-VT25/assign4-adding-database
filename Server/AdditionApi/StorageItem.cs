using System.ComponentModel.DataAnnotations;

namespace assign3_addition_api.Models
{
    public class StorageItem
    {
        [Key]  // Primary key
        public string Key { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}
