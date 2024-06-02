using System.ComponentModel.DataAnnotations.Schema;

namespace Trainning__Management.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DeadLine { get; set; }
        [ForeignKey("Training")]
        public int tr_id { get; set; }
        public Training?Training { get; set; }
    }
}
