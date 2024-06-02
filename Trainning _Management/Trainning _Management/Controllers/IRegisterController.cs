using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Trainning__Management.Data;
using Trainning__Management.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace Trainning__Management.Controllers
{
    public class IRegisterController : Controller
    {

        private readonly T_MDbContext _dbcontext;
        private readonly IHostingEnvironment _environment;

        //ihd inj 
        public IRegisterController(T_MDbContext context, IHostingEnvironment environment)
        {
            _dbcontext = context;
            _environment = environment;
        }

        //  [HttpGet]
        //public IActionResult Index(Instructor user)
        //{

        //    var instructor = _dbcontext.instructors.SingleOrDefault(i => i.Id == user.Id);
        //    if (instructor == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(new List<Instructor> { instructor });
        //}


        //inatractor
        [HttpGet]
        public IActionResult IRegister()
        {
            return View(new Instructor());
        }
        [HttpPost]
        public IActionResult IRegister(Instructor user)
        {

           
            //ModelState.Remove("clientFile");
          //  ModelState.Remove("ImageUrl");

            if (ModelState.IsValid){

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


				_dbcontext.instructors.Add(user);
                _dbcontext.SaveChanges();
                TempData["successData"] = "Instructor has been added successfully";
                return RedirectToAction("Login", "Login");
            }
            return View(user);
        }
       
       

    }
}





 
