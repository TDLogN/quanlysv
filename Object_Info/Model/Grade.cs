using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models
{
    public class Grade
    {
        [Key]
        [Display(Name = "Id Điểm")]
        public int GradeID { get; set; }
        [Display(Name = "Id Đăng Ký")]
        public int EnrollmentID { get; set; }

        [Range(0, 10)]
        [Display(Name = "Điểm giữa kỳ")]
        public double MidtermScore { get; set; }

        [Range(0, 10)]
        [Display(Name = "Điểm Cuối Kỳ")]
        public double FinalScore { get; set; }

        [Range(0, 10)]
        [Display(Name = "Điểm Trung Bình")]
        public double AverageScore { get; set; }

        [MaxLength(255)]
        [Display(Name = "Đánh giá")]
        public string GradeLetter { get; set; }
    }
}
