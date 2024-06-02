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

namespace Trainning__Management.Controllers
{
    public class TaskController : Controller
    {
        private readonly T_MDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public TaskController(T_MDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Task
        public async Task<IActionResult> Index()
        {
            var t_MDbContext = _context.tasks.Include(t => t.Training);
            return View(await t_MDbContext.ToListAsync());
        }

        public async Task<IActionResult> dIndex()
        {
            var t_MDbContext = _context.tasks.Include(t => t.Training);
            return View(await t_MDbContext.ToListAsync());
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.tasks
                .Include(t => t.Training)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            var instructorId = HttpContext.Session.GetInt32("UserId");

            // Fetch trainings where the current user is the instructor
            var trainings = _context.training.Where(t => t.In_id == instructorId.Value).ToList();

            // Create a SelectList for the ViewData
            ViewData["tr_id"] = new SelectList(trainings, "id", "name");

                        return View();
        }

        // POST: Task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,DeadLine,tr_id")] Tasks tasks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var instructorId = HttpContext.Session.GetInt32("UserId");
            // Fetch trainings where the current user is the instructor
            var trainings = _context.training.Where(t => t.In_id == instructorId.Value).ToList();

            ViewData["tr_id"] = new SelectList(trainings, "id", "name", tasks.tr_id);

            return View(tasks);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            ViewData["tr_id"] = new SelectList(_context.training, "id", "name", tasks.tr_id);
            return View(tasks);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DeadLine,tr_id")] Tasks tasks)
        {
            if (id != tasks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.Id))
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
            ViewData["tr_id"] = new SelectList(_context.training, "id", "name", tasks.tr_id);
            return View(tasks);
        }

        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(int taskId, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var task = await _context.tasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound();
                }

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Here you can update the task entity with the file path if needed
                // task.FilePath = uniqueFileName;
                // _context.Update(task);
                // await _context.SaveChangesAsync();

                // For now, we just return to the Index view
                return RedirectToAction(nameof(dIndex));
            }

            return RedirectToAction(nameof(dIndex));
        }
            //


            // GET: Task/Delete/5
            public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.tasks
                .Include(t => t.Training)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.tasks.FindAsync(id);
            if (tasks != null)
            {
                _context.tasks.Remove(tasks);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
            return _context.tasks.Any(e => e.Id == id);
        }
    }
}
