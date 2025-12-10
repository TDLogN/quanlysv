using QuanLySinhVien.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLySinhVien.Helpers
{
    public static class AccountManager
    {
        // Hàm giả lập lấy thông tin User và quyền hạn từ username
        public static UserInfo GetUserInfo(string username)
        {
            if (username.ToLower() == "admin")
            {
                return new UserInfo
                {
                    UserName = "Admin",
                    Role = "Admin",
                    GA_Confirmed = true, 
                    List_User_Functions = new List<string>
                    {
                        "Account_VIEW", "ACCOUNT_CREATE", "ACCOUNT_EDIT", "ACCOUNT_DELETE",
                        "STUDENT_VIEW", "STUDENT_CREATE", "STUDENT_EDIT", "STUDENT_DELETE",
                        "SUBJECT_VIEW", "SUBJECT_CREATE", "SUBJECT_EDIT", "SUBJECT_DELETE",
                        "CLASS_VIEW","CLASS_CREATE", "CLASS_EDIT", "CLASS_DELETE","CLASS_DETAIL",
                        "FACULTY_VIEW","FACULTY_CREATE", "FACULTY_EDIT", "FACULTY_DELETE",
                        "TEACHER_VIEW","TEACHER_CREATE", "TEACHER_EDIT", "TEACHER_DELETE",
                        "GRADE_VIEW","GRADE_CREATE", "GRADE_EDIT", "GRADE_DELETE",
                        "ROLE_VIEW","ROLE_CREATE", "ROLE_EDIT", "ROLE_DELETE"
                    }
                };
            }
            else if (username.ToLower() == "Giảng viên")
            {
                return new UserInfo
                {
                    UserName = username,
                    Role = "Giảng viên",
                    GA_Confirmed = true,
                    List_User_Functions = new List<string>
                    {
                        "STUDENT_VIEW", "SUBJECT_VIEW", "CLASS_VIEW", "GRADE_VIEW", "GRADE_EDIT"
                    }
                };
            }
            else if (username.ToLower() == "Student") 
            {
                return new UserInfo
                {
                    UserName = username,
                    Role = "Student",
                    GA_Confirmed = false,
                    List_User_Functions = new List<string>
                    {
                        "STUDENT_VIEW", "SUBJECT_VIEW", "GRADE_VIEW"
                    }
                };
            }
            else
            {
                return new UserInfo
                {
                    UserName = "User",
                    Role = "User",
                    GA_Confirmed = true,
                    List_User_Functions = new List<string>
                    {
                        "STUDENT_VIEW","SUBJECT_VIEW","GRADE_VIEW","CLASS_VIEW"
                    }
                };
            }
        }

        // Hàm kiểm tra đăng nhập mới nhất (mặc định luôn trả về true)
        public static bool IsLastestLoggedIn(UserInfo user) => true;
    }
}