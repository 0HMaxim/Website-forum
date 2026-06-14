using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Website_forum.Context;
using Website_forum.Models;
using Website_forum.Models.ViewModels;

namespace Website_forum.Controllers
{
    public class HomeController : Controller
    {
        IdentityDbContext context;
        Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        public HomeController(Microsoft.AspNetCore.Identity.UserManager<User> userManager, IdentityDbContext context)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Topics()
        {
            context.Posts.Load();
            return View(context.Topic.ToList() ?? new List<Topic>());
        }

        public IActionResult ListPosts(int? id)
        {
            var topic = context.Topic
                                .FirstOrDefault(x => x.Id == id);
            context.Topic.Load();
            context.Users.Load();
            context.Comments.Load();
            if (topic != null)
                return View(new ListPostViewModel
                {
                    Posts = context.Posts
                            .Where(x => x.Topic.Id == topic.Id)
                                .ToList(),
                    Topic = topic,

                });
            return View(new ListPostViewModel
            {
                Posts = context.Posts
                                  .OrderBy(x => x.PostDate)
                                     .ToList(),
            });
        }

        public IActionResult CreateTopic()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateTopic(string name)
        {
            if (ModelState.IsValid)
            {
                var topic = new Topic { Name = name };
                context.Topic.Add(topic);
                context.SaveChanges();
                return RedirectToAction("Topics");
            }
            return View();
        }

        public IActionResult CreatePost(int id)
        {
            context.Topic.Load();
            ViewBag.Topics = context.Topic.ToList();
            var topic = context.Topic.FirstOrDefault(x => x.Id == id);
            return View(new Post { Topic = topic, PostDate = DateTime.Now, });
        }

        [HttpPost]
        public IActionResult CreatePost(Post post)
        {
            if (post.Topic != null)
            {
                post.Topic = context.Topic.FirstOrDefault(x => x.Id == post.Topic.Id);
            }

            if (ModelState.IsValid && post.Topic != null)
            {
                post.Owner = userManager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
                context.Posts.Add(post);
                context.SaveChanges();
                return RedirectToAction("ListPosts", new { id = post.Topic.Id });
            }

            if (post.Topic == null)
            {
                ModelState.AddModelError("", "Please select a topic");
            }
            ViewBag.Topics = context.Topic.ToList();
            return View(post);
        }

        public IActionResult OpenPost(int? id)
        {
            var post = context.Posts
                .Include(p => p.Topic)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefault(x => x.Id == id);

            if (post == null)
            {
                return RedirectToAction("Topics");
            }

            return View(new CommentsPostViewModel { Post = post });
        }


        [HttpPost]
        public async Task<IActionResult> OpenPost(CommentsPostViewModel model)
        {
            context.Comments.Load();
            var post = context.Posts.Include(p => p.Topic).FirstOrDefault(x => x.Id == model.Post.Id);
            var user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (ModelState.IsValid)
            {
                var comment = new Comment { Post = post, Text = model.Answer, User = user };
                post.Comments.Add(comment);
                context.Update(post);
                context.SaveChanges();
                return RedirectToAction("OpenPost", new { id = post.Id });
            }
            ViewBag.Topics = context.Topic.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult EditComment(int id)
        {
            context.Users.Load();
            context.Topic.Load();
            context.Posts.Load();
            var comm = context.Comments.FirstOrDefault(x => x.Id == id);
            if (comm != null)
                return View(comm);
            return View(new ErrorViewModel());
        }

        [HttpPost]
        public IActionResult EditComment(Comment comment)
        {
            var comm = context.Comments.FirstOrDefault(x => x.Id == comment.Id);
            if (ModelState.IsValid)
            {
                comm.Post = context.Posts.FirstOrDefault(x => x.Id == comment.Post.Id);
                comm.User = context.Users.FirstOrDefault(x => x.Id == comment.User.Id);
                comm.Text = comment.Text;
                context.Update(comm);
                context.SaveChanges();
                return RedirectToAction("OpenPost", new { id = comm.Post.Id });
            }
            return View(comment);
        }

        [HttpPost]
        public IActionResult Delete(int id, string returnUrl)
        {
            var postToDelete = context.Posts.Include(p => p.Comments).FirstOrDefault(x => x.Id == id);
            if (postToDelete != null)
            {
                context.Comments.RemoveRange(postToDelete.Comments);
                context.Posts.Remove(postToDelete);
                context.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        [HttpPost]
        public IActionResult DeleteComment(int id, string returnUrl)
        {
            var comment = context.Comments.Include(c => c.User).FirstOrDefault(x => x.Id == id);
            if (comment != null && comment.User != null && comment.User.UserName == HttpContext.User.Identity.Name)
            {
                context.Comments.Remove(comment);
                context.SaveChanges();
            }
            return Redirect(returnUrl);
        }

    }
}