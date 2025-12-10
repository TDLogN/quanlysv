using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySinhVien.Models;
using QuanLySinhVien.Data;
using QuanLySinhVien.Filters;
using quanlysv;
using RestSharp;
using System.Text.Json;
using PagedList;
using Microsoft.AspNetCore.Authorization; 

namespace QuanLySinhVien.Controllers
{
    //[CustomActionFilter(FunctionCode = "FACULTY_VIEW", CheckAuthentication = true)]

    public class FacultyController : Controller
    {
        private readonly string apiBaseUrl = Config_Info.APIURL;

        // GET: /Faculty - Cho phép TẤT CẢ mọi người xem (Kể cả người chưa đăng nhập)
        public async Task<IActionResult> Index(string keyword, int? page)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/FacultyApi/GetAllFaculties", Method.Get);
            Console.WriteLine(client.BuildUri(request)); // debug URL

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                ViewBag.Error = "Không thể tải danh sách khoa.";
                return View(new List<Faculty>());
            }
            var faculties = JsonSerializer.Deserialize<List<Faculty>>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Nếu có từ khóa thì lọc
            if (!string.IsNullOrEmpty(keyword))
            {
                faculties = faculties
                    .Where(f => f.FacultyName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            // phan trang
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var pagedData = faculties
                .OrderBy(f => f.FacultyID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)faculties.Count / pageSize);

            return View(pagedData);
        }

        [CustomActionFilter(FunctionCode = "FACULTY_CREATE")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Faculty faculty)
        {
            if (!ModelState.IsValid)
                return View(faculty);

            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/FacultyApi/Add", Method.Post);

            request.AddJsonBody(faculty);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = $"Không thể thêm khoa mới. Chi tiết: {response.Content}";
            return View(faculty);
        }

        [CustomActionFilter(FunctionCode = "FACULTY_EDIT")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/FacultyApi/id/{id}", Method.Get);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                return NotFound();
            }
            var faculty = JsonSerializer.Deserialize<Faculty>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (faculty == null)
                return NotFound();

            return View(faculty);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Faculty faculty)
        {
            if (id != faculty.FacultyID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(faculty);

            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/FacultyApi/Edit", Method.Put);
            request.AddJsonBody(faculty);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return RedirectToAction(nameof(Index));

            ViewBag.Error = "Không thể cập nhật thông tin khoa.";
            return View(faculty);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/FacultyApi/{id}", Method.Delete);
            Console.WriteLine(client.BuildUri(request)); // debug URL

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                TempData["Success"] = "Đã xóa khoa thành công.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = $"Không thể xóa khoa. Lỗi: {response.StatusCode} - {response.Content}";
            return RedirectToAction(nameof(Index));
        }

        [CustomActionFilter(FunctionCode = "FACULTY_DELETE")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/FacultyApi/id/{id}", Method.Get);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                return NotFound();
            }
            var faculty = JsonSerializer.Deserialize<Faculty>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (faculty == null)
                return NotFound();

            return View(faculty);
        }
    }
}