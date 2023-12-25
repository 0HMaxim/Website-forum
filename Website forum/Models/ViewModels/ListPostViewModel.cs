using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website_forum.Models.ViewModels
{
    public class ListPostViewModel
    {
        public List<Post> Posts { get; set; }
        public Topic Topic { get; set; }
    }
}
