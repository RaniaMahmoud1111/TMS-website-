using System.ComponentModel.DataAnnotations.Schema;

namespace Trainning__Management.Models
{
    public class Materials
    {
        public int Id { get; set; }
        public string MaterialName { get; set; }
        public string file_path { get; set; }
        [ForeignKey("Training")]
        public int tr_id {  get; set; }
       
        public Training?Training { get; set; }

    }
}
