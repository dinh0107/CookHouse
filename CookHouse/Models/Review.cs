using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CookHouse.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Vote { get; set; }
        [Display(Name = "Nhận xét của bạn *"), Required(ErrorMessage = "Hãy nhập nhận xét của bạn"), UIHint("TextArea")]
        public string Content { get; set; }
        [Display(Name = "Họ tên*"), Required(ErrorMessage = "Hãy nhập họ tên"), UIHint("TextBox")]
        public string Fullname { get; set; }
        [Display(Name = "Email *"), EmailAddress(ErrorMessage = "Email không hợp lệ"), Required(ErrorMessage = "Hãy nhập Email"), UIHint("TextBox")]
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Review()
        {
            CreateDate = DateTime.Now;
            Active = false;
        }
    }
}