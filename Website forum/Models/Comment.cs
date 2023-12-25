using System.ComponentModel.DataAnnotations;

namespace Website_forum.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        [Required]
        public string Text { get; set; }
    }
}