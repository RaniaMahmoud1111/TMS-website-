using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning__Management.Data;
using Trainning__Management.Models;
using System;
using System.Threading.Tasks;

namespace Trainning__Management.Controllers
{
    public class AddController : Controller
    {
        private readonly T_MDbContext _context;

        public AddController(T_MDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var traineeId = HttpContext.Session.GetInt32("UserId");
            if (traineeId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var t_MDbContext = _context.training.Include(t => t.Instructor);
            return View(await t_MDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddTraining(int id)
        {
            var traineeId = HttpContext.Session.GetInt32("UserId");
            if (traineeId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var training = await _context.training.FirstOrDefaultAsync(t => t.id == id);
            if (training == null)
            {
                return NotFound();
            }

            var attendance = new attend
            {
                Train_id = training.id,
                Traine_id = traineeId.Value,
                att_day = DateTime.Now.ToShortDateString()
            };

            _context.attends.Add(attendance);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"The training '{training.name}' was added successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
