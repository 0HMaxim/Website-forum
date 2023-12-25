using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website_forum.Models
{
    public class Topic
    {
        public Topic()
        {
            Posts = new List<Post>();
        }
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
    }
}
