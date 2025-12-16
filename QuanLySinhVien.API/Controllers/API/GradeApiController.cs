using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.API.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GradeApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        // get api/grade lay danh sach diem
        [HttpGet]
        [Route("GetAllGrades")]
        public async Task<IActionResult> GetAllGrades()
        {
            var grades = await _context.Grades
                .FromSqlRaw("CALL GetAllGrades()")
                .ToListAsync();
            return Ok(grades);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGradeById(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
                return NotFound();
            return Ok(grade);
        }
        // post api/grade them diem moi
        [HttpPost("Create")]
        public async Task<IActionResult> AddGrade([FromBody] Grade grade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL AddGrade({0}, {1}, {2}, {3}, {4})",
                    grade.EnrollmentID,
                    grade.MidtermScore,
                    grade.FinalScore,
                    grade.AverageScore,
                    grade.GradeLetter
                );
                return Ok(new { message = "Thêm điểm thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Đã xảy ra lỗi khi thêm điểm.",
                    details = ex.Message    
                });
            }
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> EditGrade(int id, [FromBody] Grade model)
        {
            if (id != model.GradeID)
                return BadRequest("ma diem khong dung");
            try
            {
               int affectedRows = await _context.Database.ExecuteSqlRawAsync(
                    "CALL UpdateGrade({0}, {1}, {2}, {3}, {4})",
                    model.EnrollmentID,
                    model.MidtermScore,
                    model.FinalScore,
                    model.AverageScore,
                    model.GradeLetter
                );
                if (affectedRows > 0)
                    return Ok(new { message = "Cập nhật điểm số thành công" });

                return StatusCode(500, new { message = "Không có dòng nào bị ảnh hưởng." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật điểm số.", detail = ex.Message });
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL DeleteGrade({0})",
                    id
                );
                return Ok(new { message = " Xóa điểm thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Đã xảy ra lỗi khi xóa điểm.", details = ex.Message });
            }
        }
    }
}

