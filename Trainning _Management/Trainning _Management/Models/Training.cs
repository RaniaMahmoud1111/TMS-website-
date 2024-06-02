using System.ComponentModel.DataAnnotations.Schema;

namespace Trainning__Management.Models
{
    public class Training
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        public int period { get; set; }

        public ICollection<Materials>? materials { get; set; }

        [ForeignKey("Instructor")]
        public int In_id { get; set; }
        public Instructor? Instructor { get; set; }

        public ICollection<Tasks>? tasks { get; set; }

       

    }
}
