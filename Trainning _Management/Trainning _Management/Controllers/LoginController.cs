using Microsoft.AspNetCore.Mvc;
using Trainning__Management.Data;
using Trainning__Management.Models;

namespace Trainning__Management.Controllers
{
    public class LoginController : Controller
    {
       
        private readonly T_MDbContext _dbcontext;
        public LoginController(T_MDbContext context)
        {
            _dbcontext = context;

        }
        

        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }

		[HttpPost]
		public IActionResult Login(login model)
		{
			if (ModelState.IsValid)
			{
				var Iuser = _dbcontext.instructors.FirstOrDefault(u => u.Email == model.email);
				var Auser = _dbcontext.admins.FirstOrDefault(u => u.Email == model.email);
				var Tuser = _dbcontext.trainees.FirstOrDefault(u => u.Email == model.email);


				//Instructor
				if (Auser == null && Tuser == null)
				{
					if (Iuser == null || !BCrypt.Net.BCrypt.Verify(model.pass, Iuser.Password))
					{
						// Invalid login attempt
						ModelState.AddModelError(string.Empty, "Invalid login attempt.");
						return View();
					}
					else if (Iuser != null)
					{
						HttpContext.Session.SetInt32("UserId", Iuser.Id);
						HttpContext.Session.SetString("User", Iuser.Email);
						HttpContext.Session.SetString("img", Iuser.ImageUrl);

						return RedirectToAction("I_Index", "Home");
					}

				}

				//Admin
				if (Iuser == null && Tuser == null)
				{
					if (Auser == null || !BCrypt.Net.BCrypt.Verify(model.pass, Auser.Password))
					{
						// Invalid login attempt
						ModelState.AddModelError(string.Empty, "Invalid login attempt.");
						return View();
					}
					else if (Auser != null)
					{
						HttpContext.Session.SetInt32("UserId", Auser.Id);
						HttpContext.Session.SetString("User", Auser.Email);
						HttpContext.Session.SetString("img", Auser.ImageUrl);

						return RedirectToAction("AIndex", "Home");
					}
				}
				else
				{

					//Trainee
					if (Tuser == null || !BCrypt.Net.BCrypt.Verify(model.pass, Tuser.Password))
					{
						// Invalid login attempt
						ModelState.AddModelError(string.Empty, "Invalid login attempt.");
						return View();
					}
					else if (Tuser != null)
					{
						HttpContext.Session.SetInt32("UserId", Tuser.Id);
						HttpContext.Session.SetString("User", Tuser.Email);
						HttpContext.Session.SetString("img", Tuser.ImageUrl);

						return RedirectToAction("TIndex", "Home");
					}
				}

		}
			else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            
            return View(model);
        }



		//log out 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login","Login");
        }

    }
}
