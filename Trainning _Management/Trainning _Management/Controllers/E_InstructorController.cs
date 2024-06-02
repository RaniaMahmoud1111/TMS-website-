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
    public class E_InstructorController : Controller
    {
        private readonly T_MDbContext _context;
        private readonly IHostingEnvironment _environment;
        public E_InstructorController(T_MDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {

            var instructorId = HttpContext.Session.GetInt32("UserId");
            if (instructorId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var instructor = await _context.instructors.FindAsync(instructorId.Value);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(new List<Instructor> { instructor });
        }


        public async Task<IActionResult> Trainings()
        {
            var instructorId = HttpContext.Session.GetInt32("UserId");
            if (instructorId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var trainings = await _context.training
                                          .Where(t => t.In_id == instructorId.Value)
                                          .ToListAsync();

            return View(trainings);
        }


        // GET: E_Instructor/AddMaterial/5
        public IActionResult AddMaterial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["TrainingId"] = id.Value;
            return View();
        }

        // POST: E_Instructor/AddMaterial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMaterial(int trainingId, [Bind("MaterialName")] Materials material, IFormFile file_path)
        {
            if (file_path != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string filePath = Path.Combine(uploadsFolder, file_path.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file_path.CopyToAsync(stream);
                    material.file_path = file_path.FileName;
                }
            }
            else
            {
                material.file_path = "default.png";
            }

            ModelState.Remove("file_path");

            if (ModelState.IsValid)
            {
                material.tr_id = trainingId;
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Trainings));
            }
            ViewData["TrainingId"] = trainingId;
            return View(material);
        }

        // GET: E_Instructor/AddTask/5
        public IActionResult AddTask(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["TrainingId"] = id.Value;
            return View();
        }

        // POST: E_Instructor/AddTask
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTask(int trainingId, [Bind("Name, Description, DeadLine")] Tasks task)
        {
            if (ModelState.IsValid)
            {
                task.tr_id = trainingId;
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Trainings));
            }
            ViewData["TrainingId"] = trainingId;
            return View(task);
        }



        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit()
        {
            var instructorId = HttpContext.Session.GetInt32("UserId");
            if (instructorId == null)
            {
                return NotFound();
            }
            var instructor = _context.instructors.SingleOrDefault(i => i.Id == instructorId.Value);

            if (instructor == null)
            {
                return NotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,Phone,Level,ImageUrl,clientFile,Password,ConfPassword,Skills")] Instructor instructor)
        {
            var instructorId = HttpContext.Session.GetInt32("UserId");
            if (id != instructor.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                string fileName = string.Empty;
                if (instructor.clientFile != null)
                {
                    string myUpload = Path.Combine(_environment.WebRootPath, "img");
                    fileName = instructor.clientFile.FileName;
                    string fullPath = Path.Combine(myUpload, fileName);
                    instructor.clientFile.CopyTo(new FileStream(fullPath, FileMode.Create));
                    instructor.ImageUrl = fileName;
                }
                else
                {
                    instructor.ImageUrl = "defult.png";
                }
                try
                {

                    //hash password
                    instructor.Password = BCrypt.Net.BCrypt.HashPassword(instructor.Password);
                    instructor.ConfPassword = instructor.Password; // Ensure ConfPassword is also hashed

                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "E_Instructor");
            }
            return View(instructor);
        }
        private bool InstructorExists(int id)
        {
            return _context.instructors.Any(e => e.Id == id);
        }

        ///end
        ///




    }
}
