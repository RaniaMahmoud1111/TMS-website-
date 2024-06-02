using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning__Management.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Trainning__Management.Controllers
{
    public class View_traningController : Controller
    {
        private readonly T_MDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public View_traningController(T_MDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> view_traning()
        {
            var traineeId = HttpContext.Session.GetInt32("UserId");
            if (traineeId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var trainings = await _context.attends
                                    .Include(a => a.Training)
                                    .Where(a => a.Traine_id == traineeId.Value)
                                    .Select(a => a.Training)
                                    .ToListAsync();

            return View(trainings);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTraining(int trainingId)
        {
            var traineeId = HttpContext.Session.GetInt32("UserId");
            if (traineeId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var attendance = await _context.attends
                                           .FirstOrDefaultAsync(a => a.Train_id == trainingId && a.Traine_id == traineeId.Value);

            if (attendance != null)
            {
                _context.attends.Remove(attendance);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(view_traning));
        }

        public async Task<IActionResult> ShowMaterial(int trainingId)
        {
            var materials = await _context.materials
                                          .Where(m => m.tr_id == trainingId)
                                          .ToListAsync();

            return View(materials);
        }
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
        { ".csv", "text/csv" }
    };
        }

    }
}
