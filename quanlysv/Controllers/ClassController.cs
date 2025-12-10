using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySinhVien.Data;
using QuanLySinhVien.Filters;
using QuanLySinhVien.Models;
using quanlysv;
using RestSharp;
using System.Text.Json;

namespace QuanLySinhVien.Controllers
{
    [CustomActionFilter(FunctionCode = "CLASS_VIEW", CheckAuthentication = true)]

    public class ClassController : Controller
    {
        private readonly string apiBaseUrl = Config_Info.APIURL;
        private readonly ApplicationDbContext _context;

        public ClassController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LOAD SELECT LIST CHUNG
        private void LoadSelectList()
        {
            ViewData["FacultyList"] = new SelectList(
                _context.Faculties.ToList(), "FacultyID", "FacultyName");

            ViewData["TeacherList"] = new SelectList(
                _context.Teachers
                    .Select(t => new { t.TeacherID, FullName = t.FirstName + " " + t.LastName })
                    .ToList(),
                "TeacherID", "FullName");
        }
        [CustomActionFilter(FunctionCode = "CLASS_VIEW", CheckAuthentication = true)]

        public async Task<IActionResult> Index(string keyword, int? page)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/ClassApi/GetAllClasses", Method.Get);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                ViewBag.Error = "Không thể tải danh sách lớp.";
                return View(new List<Class>());
            }

            var classes = JsonSerializer.Deserialize<List<Class>>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Nếu có từ khóa thì lọc
            if (!string.IsNullOrEmpty(keyword))
            {
                classes = classes
                    .Where(c => c.ClassName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            //phan trang
            int pageSize = 5;
            int pageNumer = page ?? 1;

            var pagedData = classes
                .OrderBy(c => c.ClassID)
                .Skip((pageNumer - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            ViewBag.Keyword = keyword;
            ViewBag.PageNumber = pageNumer;
            ViewBag.TotalPages = (int)Math.Ceiling((double)classes.Count / pageSize);

            return View(pagedData);
        }
        [CustomActionFilter(FunctionCode = "CLASS_EDIT")]
        [HttpGet]
        public IActionResult AddClass()
        {
            LoadSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClass(Class model)
        {
            if (!ModelState.IsValid)
            {
                LoadSelectList();
                return View(model);
            }

            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/ClassApi/AddClass", Method.Post);

            request.AddJsonBody(model);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = response.Content;
            LoadSelectList();
            return View(model);
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> EditClass(int id)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/ClassApi/{id}", Method.Get);

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
                return NotFound();

            var cls = JsonSerializer.Deserialize<Class>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            LoadSelectList();
            return View(cls);
        }

        [HttpPost]
        public async Task<IActionResult> EditClass(int id, Class model)
        {
            if (id != model.ClassID)
                return BadRequest();

            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/ClassApi/EditClass/{id}", Method.Put);

            request.AddJsonBody(model);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return RedirectToAction(nameof(Index));

            LoadSelectList();
            return View(model);
        }

        // DELETE CLASS
        [CustomActionFilter(FunctionCode = "CLASS_DELETE")]


        [HttpGet]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/ClassApi/{id}", Method.Get);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                return NotFound();

            var cls = JsonSerializer.Deserialize<Class>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(cls);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/ClassApi/DeleteClass/{id}", Method.Delete);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = response.Content;
            return RedirectToAction(nameof(Index));
        }

    }
}
