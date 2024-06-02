using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trainning__Management.Models
{
   

    public class attend
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Training")]
        public int Train_id { get; set; }
        [ForeignKey("Trainee")]
        public int Traine_id { get; set; }
        public Training?Training { get; set; }
        public Trainee? Trainee { get; set; }
        public string att_day { get; set; }

        
    }
}
