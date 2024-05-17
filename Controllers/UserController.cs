using JobPortal.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPortal.Controllers
{
    public class UserController : Controller
    {
        JobPortalDbContext db_context = new JobPortalDbContext();
        // GET: UserController/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: UserController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserViewModel uvm)
        {
            try
            {
                var user_exits = db_context.Users.Any(x => x.Username == uvm.Username);
                if (!user_exits)
                {
                    var entity = new User()
                    {
                        Username = uvm.Username,
                        Password = uvm.Password,
                    };
                    db_context.Users.Add(entity);
                    db_context.SaveChanges();

                    return RedirectToAction(nameof(Login));
                }
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: UserController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var db_user = db_context.Users.Where(x =>
                x.Username == vm.Username).FirstOrDefault();
                if (db_user != null && db_user.Password == vm.Password)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, db_user.Username),
                        new Claim(ClaimTypes.NameIdentifier, db_user.Id.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties();

                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                        );
                    return RedirectToAction("Index", "Job");

                }
                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View();
        }

        // GET: UserController/Logout
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
