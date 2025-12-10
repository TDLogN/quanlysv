using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.API.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllAccounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return Ok(accounts);
        }
        

        // GET: api/AccountApi/{id}
        [HttpGet("{id}")]
        public IActionResult GetAccountById(int id)
        {
            var account = _context.Accounts
                .FromSqlRaw($"CALL GetAccountById({id})")
                .AsNoTracking()
                .AsEnumerable()
                .FirstOrDefault();

            if (account == null)
                return NotFound();

            return Ok(account);
        }

        // PUT: api/AccountApi/UpdateRole
        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest req)
        {
            var account = await _context.Accounts.FindAsync(req.AccountID);
            if (account == null)
                return NotFound();

            account.RoleID = req.RoleID;
            account.Role = req.RoleName;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật vai trò tài khoản thành công!" });
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return NotFound();
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Xóa tài khoản thành công." });
        }
    }

    public class UpdateRoleRequest
    {
        public int AccountID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

}
