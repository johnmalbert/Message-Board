using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWall.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace TheWall.Controllers
{
    public class LoginController : Controller
    {
        private MyContext _context;

        public LoginController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            //check for validation errors, ensure no two users have same email
            if(_context.Users.Any(u => u.Email == newUser.Email))
            {
                // email already exists in database
                ModelState.AddModelError("Email", "Email address is already in use.");
            }
            if(ModelState.IsValid)
            {
                //hash the password
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                //add user to db
                _context.Add(newUser);
                _context.SaveChanges();

                //store id in session
                HttpContext.Session.SetInt32("UserId", newUser.UserId);

                // redirect to dashboard
                return RedirectToAction("Dashboard");
            }
            
            return View("Index");
            
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userToLogin)
        {
            // check if user exists in db
            var foundUser = _context.Users
                .FirstOrDefault(u => u.Email == userToLogin.LoginEmail);

            if(foundUser == null)
            {
                ModelState.AddModelError("LoginEmail", "Please check your email and password.");
                return View("Index");
            }
            // check password match
            var hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(userToLogin, foundUser.Password, userToLogin.LoginPassword);

            if(result == 0)
            {
                ModelState.AddModelError("LoginEmail", "Please check your email and password.");
                return View("Index");
            }

            HttpContext.Session.SetInt32("UserId", foundUser.UserId);
            return RedirectToAction("Dashboard");

            // check validation
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            //get the user's info from session
            int? userId = HttpContext.Session.GetInt32("UserId");

            //check to ensure logged in
            if(userId == null)
            {
                return RedirectToAction("Index");
            }
            // pass user info to the view
            User CurrentUser = _context.Users
                .First(u => u.UserId == userId);

            //show the previous posts and comments in descending order.
            List<Message> AllMessages = _context.Messages
                .Include(m => m.Creator)
                .Include(m => m.CommentsOnPost)
                .ThenInclude(c => c.UserWhoCommented)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            ViewBag.AllMessages = AllMessages;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        public User CheckLogin()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return null;
            }
            // pass user info to the view
            User CurrentUser = _context.Users
                .First(u => u.UserId == UserId);
            return (CurrentUser);
        }

        [HttpPost("postmessage")]
        public IActionResult PostMessage(Message newMessage)
        {
            Console.WriteLine(newMessage.MessageBody);
            //get user who is logged in
            User CurrentUser = CheckLogin();
            if(CurrentUser == null)
                return RedirectToAction("Index");
            
            //set the message creator
            if(ModelState.IsValid)
            {
                newMessage.Creator = CurrentUser;
                _context.Add(newMessage);
                _context.SaveChanges();               
                return RedirectToAction("Dashboard");
            }
            List<Message> AllMessages = _context.Messages
                .Include(m => m.Creator)
                .Include(m => m.CommentsOnPost)
                .ThenInclude(c => c.UserWhoCommented)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            ViewBag.AllMessages = AllMessages;
            ViewBag.CurrentUser = CurrentUser;
            return View("Dashboard");
        }

        [HttpPost("postcomment")]
        public IActionResult PostComment(Comment newComment)
        {
            //get logged in user
            User CurrentUser = CheckLogin();
            if(CurrentUser == null)
                return RedirectToAction("Index");
            Console.WriteLine(newComment.CommentBody);
            //check if comment is valid
            if(ModelState.IsValid)
            {
                // assigned comment to logged in user
                newComment.UserId = CurrentUser.UserId;
                //add comment to db
                _context.Add(newComment);
                _context.SaveChanges();
                Console.WriteLine("New comment added to db.");
                return RedirectToAction("Dashboard");
            }
            //redirect to dashboard
            // show the previous posts and comments in descending order.
            List<Message> AllMessages = _context.Messages
                .Include(m => m.Creator)
                .Include(m => m.CommentsOnPost)
                .ThenInclude(c => c.UserWhoCommented)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            ViewBag.AllMessages = AllMessages;
            ViewBag.CurrentUser = CurrentUser;
            return View("Dashboard");
        }

        [HttpGet("delete/{num}")]
        public IActionResult DeleteMessage(int num)
        {
            //get the message and current user
            User CurrentUser = CheckLogin();
            if(CurrentUser == null)
                return RedirectToAction("Index");
            //remove message from db
            Message MessageToDelete = _context.Messages.First(m => m.MessageId == num);

            _context.Remove(MessageToDelete);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Console.WriteLine("Clearing Session.");
            return RedirectToAction("Index");
        }
    }
}
