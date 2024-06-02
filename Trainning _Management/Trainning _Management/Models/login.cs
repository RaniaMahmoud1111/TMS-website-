using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Trainning__Management.Models
{
    public class login
    {
        [DefaultValue(0)]
        [Key]
        public int Id { get; set; }
        public string email { get; set; }

        [Required(ErrorMessage = "Please Enter password")]
        public string pass { get; set; }
    }
}
