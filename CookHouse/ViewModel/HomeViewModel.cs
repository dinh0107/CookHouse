using CookHouse.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CookHouse.ViewModel
{
    public class HeaderViewModel
    {
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
        public IEnumerable<ArticleCategory> Introduct { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
    }
    public class FooterViewModel
    {
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
    }
    public class HomeViewModel
    {
        public IEnumerable<Banner> Banners { get; set; }   
        public  IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Feedback> Feedbacks { get; set; }
    }
    public class CategoryProductViewModel
    {
        public ProductCategory Category { get; set; }
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public string Url { get; set; }
        public string Sort { set; get; }
        public int BeginCount { get; set; }
        public int EndCount { get; set; }
    }
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
    }
    public class ArticleViewModel
    {
        public PagedList.IPagedList<Article> Articles { get; set;}
        public ArticleCategory ArticleCategory { get; set; }
    }
    public class MenuArticleViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
    }
    public class ArticleDetailViewModel
    {
        public Article Article { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
    public class SearchViewModel
    {
        public IPagedList<Product> Products { get; set;}
        public string Keywords { get; set; }
    }
}