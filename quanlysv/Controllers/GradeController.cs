using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;
using QuanLySinhVien.Filters;
using quanlysv;
using RestSharp;
using System.Text.Json;

[CustomActionFilter(FunctionCode = "GRADE_VIEW", CheckAuthentication = true)]
public class GradeController : Controller
    {
    private readonly string apiBaseUrl = Config_Info.APIURL;

    private readonly ApplicationDbContext _context;
    public GradeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string keyword, int? page)
    {
        var client = new RestClient(apiBaseUrl);
        var request = new RestRequest("api/GradeApi/GetAllGrades", Method.Get);
       
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            ViewBag.Error = "Không thể tải danh sách điểm.";
            return View(new List<Grade>());
        }
        var grades = JsonSerializer.Deserialize<List<Grade>>(response.Content!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Grade>();
        //phan trang 
        int pageSize = 5;
        int pageNumber = (page ?? 1);

        var pagedGrades = grades
            .OrderBy(g => g.GradeID)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.PageNumber = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling((double)grades.Count / pageSize);
        return View(grades);
    }
    [CustomActionFilter(FunctionCode = "GRADE_CREATE")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
       return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Grade grade)
    {
        if (!ModelState.IsValid)
            return View(grade);

        var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/GradeApi/Create", Method.Post);

            request.AddJsonBody(grade);
            var response = await client.ExecuteAsync(request);
            if(response.IsSuccessful)
                return RedirectToAction(nameof(Index));
        ViewBag.Error = "Lỗi khi tạo điểm: {response.Content}";
        return View(grade);
    }

    [CustomActionFilter(FunctionCode = "GRADE_EDIT")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var client = new RestClient(apiBaseUrl);
        var request = new RestRequest($"api/GradeApi/{id}", Method.Get);
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
            return NotFound();
        
        var grade = JsonSerializer.Deserialize<Grade>(response.Content!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (grade == null)
        
            return NotFound();
        
        return View(grade);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Grade grade)
    {
        if (id != grade.GradeID)
            return NotFound();

        if (!ModelState.IsValid)
            return View(grade);

        var client = new RestClient(apiBaseUrl);
        var request = new RestRequest($"api/GradeApi/Edit/{id}", Method.Put);
        request.AddJsonBody(grade);

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
            return RedirectToAction(nameof(Index));

        ViewBag.Error = $"Lỗi khi cập nhật điểm: {response.Content}";
        return View(grade);
    }

    [CustomActionFilter(FunctionCode = "GRADE_DELETE")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var client = new RestClient(apiBaseUrl);
        var request = new RestRequest($"api/GradeApi/{id}", Method.Get);
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            return NotFound();
        }
        var grade = JsonSerializer.Deserialize<Grade>(response.Content!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return View(grade);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = new RestClient(apiBaseUrl);
        var request = new RestRequest($"api/GradeApi/Delete/{id}", Method.Delete);
        var response = await client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ViewBag.Error = "Lỗi khi xóa điểm.";
            return RedirectToAction(nameof(Index));
        }
    }

}

