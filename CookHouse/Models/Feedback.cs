using System.ComponentModel.DataAnnotations;
namespace CookHouse.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        [Display(Name = "Họ tên", Description = "Tên dài tối đa 100 ký tự"),
       Required(ErrorMessage = "Hãy nhập Họ tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Chức vụ", Description = "Dài tối đa 100 ký tự"),
         StringLength(100, ErrorMessage = "Tối đa 100 ký tự"),
         UIHint("TextBox")]
        public string Classify { get; set; }
        [Display(Name = "Lời nhận xét"), StringLength(700, ErrorMessage = "Tối đa 700 ký tự"), UIHint("TextArea")]
        public string Content { get; set; }

        [Display(Name = "Ảnh đại diện"), UIHint("ImageFeedback")]
        public string Image { get; set; }

        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        public Feedback()
        {
            Active = true;
            Sort = 1;
        }
    }
}