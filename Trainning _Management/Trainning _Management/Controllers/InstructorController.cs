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
    public class InstructorController : Controller
    {
        private readonly T_MDbContext _context;
        private readonly IHostingEnvironment _environment;
        public InstructorController(T_MDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Instructor
        public async Task<IActionResult> Index()
        {
            return View(await _context.instructors.ToListAsync());
        }

        // GET: Instructor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.instructors == null)
            { return NotFound(); }
            var instructor = await _context.instructors
               .FirstOrDefaultAsync(m => m.Id == id);

            List<Training> courses = _context.training.Where(m => m.In_id == id).ToList();

            if (instructor == null)
            {
                return NotFound();
            }
            return View(instructor);

            if (id == null)
            {
                return NotFound();
            }

         
        }

      

        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,Phone,Level,ImageUrl,clientFile,Password,ConfPassword,Skills")] Instructor instructor)
        {
            var instructorId = HttpContext.Session.GetInt32("InstructorId");
            if (id != instructor.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                if (instructor.clientFile != null)
                {
                    string myUpload = Path.Combine(_environment.WebRootPath, "img");
                    string fileName = instructor.clientFile.FileName;
                    string fullPath = Path.Combine(myUpload, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await instructor.clientFile.CopyToAsync(stream);
                    }
                    instructor.ImageUrl = fileName;
                }
                else
                {
                    // Retain the existing ImageUrl if no new file is uploaded
                    var existingInstructor = await _context.instructors.AsNoTracking().FirstOrDefaultAsync(i => i.Id == instructorId.Value);
                    if (existingInstructor != null)
                    {
                        instructor.ImageUrl = existingInstructor.ImageUrl;
                    }
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
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.instructors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.instructors.Remove(instructor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.instructors.Any(e => e.Id == id);
        }
       
    }
}
