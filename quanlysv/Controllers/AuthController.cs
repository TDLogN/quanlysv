using Microsoft.AspNetCore.Authentication; 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;
using RestSharp;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace quanlysv.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            string hash = HashPassword(password);

            var user = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == username && a.PasswordHash == hash && a.IsActive);

            if (user == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác.";
                return View();
            }

            // Tạo claims (đảm bảo role được nhận diện)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.AccountID.ToString()),
                new Claim(ClaimTypes.Role, user.Role) // rất quan trọng
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            // Lưu session nếu muốn
            HttpContext.Session.SetInt32("UserID", user.AccountID);
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {

            return View();
        }

        // ... (Hàm HashPassword)

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View(new AccountCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra tên đăng nhập đã tồn tại
            var existingUser = await _context.Accounts
             .FirstOrDefaultAsync(a => a.Username == model.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
                return View(model);
            }

            // Mã hóa mật khẩu
            var hashedPassword = HashPassword(model.Password);

            // Thiết lập Role và RoleID mặc định
            string userRole = model.Role ?? "User";
            // Giả định RoleID = 1 là "Admin", 2 là "User"
            int defaultRoleId = (userRole == "Admin") ? 1 : 2;

            // Tạo Entity Account và lưu vào DB
            var account = new Account
            {
                Username = model.Username,
                PasswordHash = hashedPassword,
                Role = userRole,
                RoleID = defaultRoleId, // Cần điều chỉnh theo logic DB của bạn
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Chuyển hướng về trang Login để người dùng đăng nhập
            TempData["SuccessMessage"] = "Tạo tài khoản thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
        // Hàm hash mật khẩu (SHA256)
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}