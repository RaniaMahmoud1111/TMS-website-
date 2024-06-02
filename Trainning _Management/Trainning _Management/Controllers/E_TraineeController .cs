using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Trainning__Management.Data;
using Trainning__Management.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace Trainning__Management.Controllers
{
    public class E_TraineeController : Controller
    {
        private readonly T_MDbContext _context;
        private readonly IHostingEnvironment _environment;
        public E_TraineeController(T_MDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {

            var traineeId = HttpContext.Session.GetInt32("UserId");
            if (traineeId == null)
            {
                return RedirectToAction("Login", "Login");
            }
          
            var trainee = await _context.trainees.FindAsync(traineeId.Value);
            if (trainee == null)
            {
                return NotFound();
            }
              return View(new List<Trainee> { trainee });
            
        }



        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit()
        {
            var traineeId = HttpContext.Session.GetInt32("UserId");
            if (traineeId == null)
            {
                return NotFound();
            }
            var trainee = _context.trainees.SingleOrDefault(i => i.Id == traineeId.Value);

            if (trainee == null)
            {
                return NotFound();
            }
            return View(trainee);
        }

        // POST: Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,Phone,Level,ImageUrl,clientFile,Password,ConfPassword,GPA")] Trainee trainee)
        {
            var traineeId = HttpContext.Session.GetInt32("UserId");
            if (id != trainee.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if (trainee.clientFile != null)
                {
                    string myUpload = Path.Combine(_environment.WebRootPath, "img");
                    fileName = trainee.clientFile.FileName;
                    string fullPath = Path.Combine(myUpload, fileName);
                    trainee.clientFile.CopyTo(new FileStream(fullPath, FileMode.Create));
                    trainee.ImageUrl = fileName;
                }
                else
                {
                    trainee.ImageUrl = "defult.png";
                }
                try
                {
                    //hash password
                    trainee.Password = BCrypt.Net.BCrypt.HashPassword(trainee.Password);
                    trainee.ConfPassword = trainee.Password; // Ensure ConfPassword is also hashed

                    _context.Update(trainee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(trainee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "E_Trainee");
            }
            return View(trainee);
        }

        private bool InstructorExists(int id)
        {
            return _context.trainees.Any(e => e.Id == id);
        }
    }
}
