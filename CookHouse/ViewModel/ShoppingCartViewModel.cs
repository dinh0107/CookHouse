using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CookHouse.Models;

namespace CookHouse.ViewModel
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<CartItem> CartItems { get; set; }
        public int? Count { get; set; }
        public Order Order { get; set; }
        public SelectList CitySelectList { get; set; }
        public SelectList DistrictSelectList { get; set; }
        [Display(Name = "Thành phố"), Required(ErrorMessage = "Bạn hãy chọn thành phố")]
        public int? CityId { get; set; }
        [Display(Name = "Quận huyện"), Required(ErrorMessage = "Bạn hãy chọn quận huyện")]
        public int? DistrictId { get; set; }
        [Display(Name = "Phường xã")]
        public int? WardId { get; set; }
        public class CartItem
        { 
            public Cart CartItems { get; set; }
            public string Size { get; set; }
        }
        public ShoppingCartViewModel()
        {
            DistrictSelectList = new SelectList(new List<District>(), "Id", "Name");
        }
        public decimal CartTotal { get; set; }
    }
    public class ShoppingCartRemoveViewModel
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int Status { get; set; }
        public int DeleteId { get; set; }
    }
    public class CartItem 
    { 
        public int Quantity { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
    }

    public class CheckOutViewModel
    {
        public Order Order { get; set; }
        public decimal CartTotal { get; set; }
        public string Gender { get; set; }

        [Display(Name = "Thành phố"), Required(ErrorMessage = "Bạn hãy chọn thành phố")]
        public int? CityId { get; set; }
        [Display(Name = "Quận huyện"), Required(ErrorMessage = "Bạn hãy chọn quận huyện")]
        public int? DistrictId { get; set; }
        [Display(Name = "Phường xã")]
        public int? WardId { get; set; }

        public SelectList CitySelectList { get; set; }
        public SelectList DistrictSelectList { get; set; }
        public SelectList WardSelectList { get; set; }

        public SelectList SelectTransport { get; set; }
        public SelectList SelectTypePay { get; set; }
        public SelectList SelectGender { get; set; }

        public IEnumerable<CartItem> CartItems { get; set; }

        public class CartItem
        {
            public Cart CartItems { get; set; }
            public string Size { get; set; }
        }
        public CheckOutViewModel()
        {
            var selectTransport = new Dictionary<int, string> { { 1, "Đến địa chỉ người nhận" }, { 2, "Khách đến nhận hàng" }, { 3, "Qua bưu điện" }, { 4, "Hình thức khác" } };
            var typePay = new Dictionary<int, string> { { 1, "Tiền mặt" }, { 2, "Chuyển khoản" } };
            //{ 3, "Thanh toán online qua VNPAY" },
            var gender = new Dictionary<string, string> { { "Nam", "Nam" }, { "Nữ", "Nữ" } };
            SelectTransport = new SelectList(selectTransport, "Key", "Value");
            SelectTypePay = new SelectList(typePay, "Key", "Value");
            SelectGender = new SelectList(gender, "Key", "Value");

            DistrictSelectList = new SelectList(new List<District>(), "Id", "Name");
            WardSelectList = new SelectList(new List<Ward>(), "Id", "Name");
        }
    }

    public class CartSummaryViewModel
    {
        public List<Cart> Carts { get; set; }
        public decimal TotalMoney { get; set; }
        public int Count { get; set; }
    }
    public class CartStatistic
    {
        public int Status { get; set; }
        public decimal totalMoney { get; set; }
        public int totalItem { get; set; }
        public int itemCont { get; set; }
        public decimal totalMoneyItem { get; set; }
        public string totalMoneyString => totalMoney.ToString("N0");
        public string total => totalMoneyItem.ToString("N0");
        public string Msg { get; set; }
    }
}