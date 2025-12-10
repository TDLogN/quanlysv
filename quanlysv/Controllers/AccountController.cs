using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Filters;
using QuanLySinhVien.Models;
using quanlysv;
using RestSharp;
using System.Text.Json;

namespace QuanLySinhVien.Controllers
{
    [CustomActionFilter(FunctionCode = "Account_VIEW", CheckAuthentication = true)]

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string apiBaseUrl = Config_Info.APIURL;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Index
        public async Task<IActionResult> Index(string keyword, int? page)
        {
            // Call API
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/AccountApi/GetAllAccounts", Method.Get);
            Console.WriteLine(client.BuildUri(request));
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
                return View(new List<Account>());

            var accounts = JsonSerializer.Deserialize<List<Account>>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<Account>();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                accounts = accounts
                    .Where(a => a.Username != null &&
                                a.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var pagedData = accounts
                .OrderBy(a => a.AccountID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)accounts.Count / pageSize);
            ViewBag.PageSize = pageSize;
            return View(pagedData);
        }
        // Action này xử lý GET /Account/Permission/{id} (id là AccountID)
        [HttpGet]
        public async Task<IActionResult> Permission(int id)
        {
            // 1. Lấy account theo ID
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return NotFound();

            // 2. Gọi API lấy danh sách Role
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/RoleApi/GetAllRoles", Method.Get);

            var response = await client.ExecuteAsync(request);

            List<Role> roles = new List<Role>();

            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                roles = JsonSerializer.Deserialize<List<Role>>(response.Content!,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Role>();
            }
            // Đưa dữ liệu ra View
            ViewBag.Account = account;
            ViewBag.Roles = roles;

            return View();
        }
        // POST: Account/UpdateRole
        [HttpPost]
        public async Task<IActionResult> UpdateRole(int accountId, int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                TempData["Error"] = "Role không tồn tại!";
                return RedirectToAction("Permission", new { id = accountId });
            }

            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest("api/AccountApi/UpdateRole", Method.Put);

            request.AddJsonBody(new
            {
                AccountID = accountId,
                RoleID = role.RoleID,
                RoleName = role.RoleName
            });

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                TempData["Success"] = "Phân quyền thành công!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Lỗi khi lưu phân quyền!";
            return RedirectToAction("Permission", new { id = accountId });
        }

        [CustomActionFilter(FunctionCode = "ACCOUNT_DELETE")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Lấy thông tin account theo ID
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/AccountApi/{id}", Method.Get);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
                return NotFound();

            var account = JsonSerializer.Deserialize<Account>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = new RestClient(apiBaseUrl);
            var request = new RestRequest($"api/AccountApi/{id}", Method.Delete);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return RedirectToAction(nameof(Index));

            TempData["Error"] = "Không thể xóa tài khoản.";
            return RedirectToAction(nameof(Index));
        }

    }
}

