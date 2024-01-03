using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CookHouse.Models;

namespace CookHouse.ViewModel
{
    public class BannerViewModel
    {
        public Banner Banner { get; set; }
        public SelectList SelectGroup { get; set; }
        public BannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner Slide" },
                { 2, "Giới thiệu" },
                { 3, "Lý do chọn chúng tôi" },
                { 4, "Đối tác" },
                { 5, "Chính sách" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
    public class ListBannerViewModel
    {
        public PagedList.IPagedList<Banner> Banners { get; set; }
        public int? GroupId { get; set; }
        public SelectList SelectGroup { get; set; }
        public ListBannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner Slide" },
                { 2, "Giới thiệu" },
                { 3, "Lý do chọn chúng tôi" },
                { 4, "Đối tác" },
                { 5, "Chính sách" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
}