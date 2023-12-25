using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website_forum.Models
{
    public class Post
    {
        public int? Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Question { get; set; }
        public DateTime PostDate { get; set; }
        public User Owner { get; set; }
        
        [Required]
        public Topic Topic { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
