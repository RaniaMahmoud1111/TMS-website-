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
    public class TraineeController : Controller
    {
        private readonly T_MDbContext _context;
        private readonly IHostingEnvironment _environment;
        public TraineeController(T_MDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Trainee
        public async Task<IActionResult> Index()
        {
            return View(await _context.trainees.ToListAsync());
        }

        // GET: Trainee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await _context.trainees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }

       

        // GET: Trainee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await _context.trainees.FindAsync(id);
            if (trainee == null)
            {
                return NotFound();
            }
            return View(trainee);
        }

        // POST: Trainee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,Phone,Level,ImageUrl,clientFile,Password,ConfPassword,GPA")] Trainee trainee)
        {
            var traineeId = HttpContext.Session.GetInt32("TraineeId");
            if (id != trainee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (trainee.clientFile != null)
                {
                    string myUpload = Path.Combine(_environment.WebRootPath, "img");
                    string fileName = trainee.clientFile.FileName;
                    string fullPath = Path.Combine(myUpload, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await trainee.clientFile.CopyToAsync(stream);
                    }
                    trainee.ImageUrl = fileName;
                }
                else
                {
                    // Retain the existing ImageUrl if no new file is uploaded
                    var existingInstructor = await _context.instructors.AsNoTracking().FirstOrDefaultAsync(i => i.Id == traineeId.Value);
                    if (existingInstructor != null)
                    {
                        trainee.ImageUrl = existingInstructor.ImageUrl;
                    }
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
                    if (!TraineeExists(trainee.Id))
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
            return View(trainee);
        }

        // GET: Trainee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainee = await _context.trainees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }

        // POST: Trainee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainee = await _context.trainees.FindAsync(id);
            if (trainee != null)
            {
                _context.trainees.Remove(trainee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TraineeExists(int id)
        {
            return _context.trainees.Any(e => e.Id == id);
        }
    }
}
