using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(20)]
        public string Email { get; set; }
    }
}
