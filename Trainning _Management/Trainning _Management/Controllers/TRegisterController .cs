using Microsoft.AspNetCore.Mvc;
using Trainning__Management.Data;
using Trainning__Management.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace Trainning__Management.Controllers
{
    public class TRegisterController : Controller
    {

        private readonly T_MDbContext _dbcontext;
        private readonly IHostingEnvironment _environment;

        public TRegisterController(T_MDbContext context, IHostingEnvironment environment)
        {
            _dbcontext = context;
            _environment = environment;

        }

        //public IActionResult Index(Trainee user)
        //{
        //    var trainee = _dbcontext.trainees.SingleOrDefault(i => i.Id == user.Id);
        //    if (trainee == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(new List<Trainee> { trainee });
            
        //}
       
       
       
        
      
        //trainee
       
        
        [HttpGet]
        public IActionResult TRegister()
        {
            return View(new Trainee());
        }

        [HttpPost]
        public IActionResult TRegister(Trainee user)
        {
            
            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if (user.clientFile != null)
                {
                    string myUpload = Path.Combine(_environment.WebRootPath, "img");
                    fileName = user.clientFile.FileName;
                    string fullPath = Path.Combine(myUpload, fileName);
                    user.clientFile.CopyTo(new FileStream(fullPath, FileMode.Create));
                    user.ImageUrl = fileName;
                }
                else
                {
                    user.ImageUrl = "defult.png";
                }

				//hash password
				user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
				user.ConfPassword = user.Password; // Ensure ConfPassword is also hashed


				_dbcontext.trainees.Add(user);
                _dbcontext.SaveChanges();
                TempData["successData"] = "Instructor has been added successfully";
                return RedirectToAction("Login", "Login");
            
        }

            return View(user);
        }

        
             


    }
}
