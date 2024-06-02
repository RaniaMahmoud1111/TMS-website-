using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Trainning__Management.Data;
using Trainning__Management.Models;

namespace Trainning__Management.Controllers
{
    public class MaterialController : Controller
    {
        private readonly T_MDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MaterialController(T_MDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Material
        //public async Task<IActionResult> Index()
        //{
        //    var t_MDbContext = _context.materials.Include(m => m.Training);
        //    return View(await t_MDbContext.ToListAsync());
        //}



        public async Task<IActionResult> Index()
        {
            // Get the logged-in instructor's ID from the session
            var instructorId = HttpContext.Session.GetInt32("UserId");
            if (instructorId == null)
            {
                return NotFound();
            }

            // Get the training sessions that the logged-in instructor teaches
            var instructorTrainings = await _context.instructors
                .Where(i => i.Id == instructorId.Value)
                .SelectMany(i => i.training)
                .Select(t => t.In_id)
                .ToListAsync();

            // Get the materials for those training sessions
            var materials = await _context.materials
                .Include(m => m.Training)
                .Where(m => instructorTrainings.Contains(m.tr_id))
                .ToListAsync();

            return View(materials);
        }

        public async Task<IActionResult> dIndex()
        {
            var t_MDbContext = _context.materials.Include(m => m.Training);
            return View(await t_MDbContext.ToListAsync());
        }
        // GET: Material/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materials = await _context.materials
                .Include(m => m.Training)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materials == null)
            {
                return NotFound();
            }

            return View(materials);
        }

        // GET: Material/Create
        public IActionResult Create()
        {
            var instructorId = HttpContext.Session.GetInt32("UserId");

            // Fetch trainings where the current user is the instructor
            var trainings = _context.training.Where(t => t.In_id == instructorId.Value).ToList();

            // Create a SelectList for the ViewData
            ViewData["tr_id"] = new SelectList(trainings, "id", "name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaterialName,tr_id")] Materials materials, IFormFile file_path)
        {
            if (file_path != null)
            {
                string imgFolderPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(imgFolderPath))
                {
                    Directory.CreateDirectory(imgFolderPath);
                }
                string imgPath = Path.Combine(imgFolderPath, file_path.FileName);
                materials.file_path = file_path.FileName;
                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    await file_path.CopyToAsync(stream);
                }
            }
            else
            {
                materials.file_path = "default.png";
            }

            ModelState.Remove("file_path");

            if (ModelState.IsValid)
            {
                _context.Add(materials);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var instructorId = HttpContext.Session.GetInt32("UserId");
            // Fetch trainings where the current user is the instructor
            var trainings = _context.training.Where(t => t.In_id == instructorId.Value).ToList();

            ViewData["tr_id"] = new SelectList(trainings, "id", "name", materials.tr_id);

            return View(materials);
        }

        // GET: Material/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materials = await _context.materials.FindAsync(id);
            if (materials == null)
            {
                return NotFound();
            }
            ViewData["tr_id"] = new SelectList(_context.training, "id", "name", materials.tr_id);
            return View(materials);
        }

      //  POST: Material/Edit/5
       //  To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaterialName,file_path,tr_id")] Materials materials, IFormFile file_path)
        {
            if (id != materials.Id)
            {
                return NotFound();
            }


            if (file_path != null)
            {
                string imgFolderPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(imgFolderPath))
                {
                    Directory.CreateDirectory(imgFolderPath);
                }
                string imgPath = Path.Combine(imgFolderPath, file_path.FileName);
                materials.file_path = file_path.FileName;
                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    file_path.CopyTo(stream);
                    materials.file_path = file_path.FileName;
                }
            }
            else
            {
                materials.file_path = "defult.png";
            }
            ModelState.Remove("file_path");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materials);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialsExists(materials.Id))
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
            ViewData["tr_id"] = new SelectList(_context.training, "id", "name", materials.tr_id);
            return View(materials);
        }

        //
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return Content("Filename is not available");
            }

            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), fileName);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
    {
        { ".txt", "text/plain" },
        { ".pdf", "application/pdf" },
        { ".doc", "application/vnd.ms-word" },
        { ".docx", "application/vnd.ms-word" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".png", "image/png" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".gif", "image/gif" },
        { ".csv", "text/csv" }
    };
        }

        //



        //GET: Material/Delete/5


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materials = await _context.materials
                .Include(m => m.Training)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materials == null)
            {
                return NotFound();
            }

            return View(materials);
        }

        // POST: Material/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materials = await _context.materials.FindAsync(id);
            if (materials != null)
            {
                _context.materials.Remove(materials);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialsExists(int id)
        {
            return _context.materials.Any(e => e.Id == id);
        }
    }
}
