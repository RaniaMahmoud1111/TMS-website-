using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trainning__Management.Models
{
    public class Instructor
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Please Enter Your name")]
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set;}
        [Phone]
        public string Phone { get; set;}
        public string Level { get; set; }
        [Display(Name ="Image")]
        [DefaultValue("defult.png")]
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile clientFile { get; set; }

        [Required(ErrorMessage = "Please Enter password")]
        [StringLength(100, ErrorMessage = "please enter at least 6 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfPassword { get; set; }
        public string Skills { get; set; }

        
        public ICollection<Training>? training { get; set; }


        
    }
}
