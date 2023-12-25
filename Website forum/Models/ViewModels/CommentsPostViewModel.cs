using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website_forum.Models.ViewModels
{
    public class CommentsPostViewModel
    {
        public Post Post { get; set; }
        [Required]
        public string Answer { get; set; }
    }
}
