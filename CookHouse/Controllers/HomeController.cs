using CookHouse.DAL;
using CookHouse.Models;
using CookHouse.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace CookHouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];

        private IEnumerable<ArticleCategory> ArticleCategories() =>
            _unitOfWork.ArticleCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<ProductCategory> ProductCategories() =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));

        [ChildActionOnly]
        public PartialViewResult Header()
        {
            var intro = ArticleCategories().Where(a => a.TypePost == TypePost.Introduct && a.ShowMenu);
            var articlesCats = ArticleCategories().Where(a => a.TypePost == TypePost.Article && a.ShowMenu);
            var productCats = ProductCategories().Where(a => a.ShowMenu);
            var model = new HeaderViewModel
            {
                Introduct = intro,
                ArticleCategories = articlesCats,
                ProductCategories = productCats
            };
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var productCategory = ProductCategories().Where(a => a.ShowFooter);
            var article = ArticleCategories().Where(a => a.ShowFooter);
            var model = new FooterViewModel
            {
                ArticleCategories = article,
                ProductCategories = productCategory
            };
            return PartialView(model);
        }
        public ActionResult Index()
        {
            var banners = _unitOfWork.BannerRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
            var cats = ProductCategories().Where(a => a.Home);
            var articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Home, o => o.OrderByDescending(a => a.CreateDate));
            var feedbacks = _unitOfWork.FeedbackRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
            var model = new HomeViewModel
            {
                Banners = banners,
                ProductCategories = cats,
                Articles = articles,
                Feedbacks = feedbacks
            };
            return View(model);
        }
        public PartialViewResult GetProductHome(int catId = 0)
        {
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Home, o => o.OrderByDescending(a => a.CreateDate));
            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId || a.ProductCategory.ParentId == catId);
            }
            return PartialView(products);
        }
        #region Articles 
        [Route("blogs")]
        public ActionResult AllArticles(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.ArticleCategory.TypePost == TypePost.Article, o => o.OrderByDescending(a => a.CreateDate));
            var model = new ArticleViewModel
            {
                Articles = articles.ToPagedList(pageNumber, pageSize),
            };
            return View(model);
        }
        [Route("blogs/{url:regex(^(?!.*(vcms|uploader|article|banner|contact|product|user)).*$)}", Order = 2)]
        public ActionResult ArticleCategory(string url, int? page)
        {
            var pageNumber = page ?? 1;
            var cateogory = _unitOfWork.ArticleCategoryRepository.GetQuery(a => a.CategoryActive && a.Url == url).FirstOrDefault();
            if (cateogory == null)
            {
                return RedirectToAction("Index");
            }
            var articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && (a.ArticleCategoryId == cateogory.Id || a.ArticleCategory.ParentId == cateogory.Id), o => o.OrderByDescending(a => a.CreateDate));
            if (articles.Count() == 1)
            {
                var fi = articles.First();
                return RedirectToAction("ArticleDetail", new { url = fi.Url });
            }
            var model = new ArticleViewModel
            {
                ArticleCategory = cateogory,
                Articles = articles.ToPagedList(pageNumber, 12)
            };
            return View(model);
        }
        [Route("blogs/{url}.html", Order = 1)]
        public ActionResult ArticleDetail(string url)
        {
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (article == null)
            {
                return RedirectToAction("Index");
            }
            var articles = _unitOfWork.ArticleRepository.GetQuery(p =>
               p.Active && (p.ArticleCategoryId == article.ArticleCategoryId && p.Id != article.Id), q => q.OrderByDescending(a => a.CreateDate), 6);
            var model = new ArticleDetailViewModel
            {
                Article = article,
                Articles = articles
            };
            return View(model);
        }
        public PartialViewResult MenuArticle()
        {
            var model = new MenuArticleViewModel
            {
                Articles = _unitOfWork.ArticleRepository.GetQuery(l => l.Active, a => a.OrderByDescending(c => c.CreateDate), 5),
            };
            return PartialView(model);
        }
        public PartialViewResult MenuLeftArticle()
        {
            var cats = ArticleCategories().Where(a => a.TypePost == TypePost.Article);
            return PartialView(cats);
        }
        #endregion
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Contact(Contact model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            var IP = HttpContext.Request.UserHostAddress;
            if (IP == null)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            DateTime currentDate = DateTime.Now.Date;
            var count = _unitOfWork.ContactRepository.GetQuery(a => a.IP == IP && DbFunctions.TruncateTime(a.CreateDate) == currentDate).Count();
            if (count > 5)
            {
                return Json(new { status = false, msg = "Vượt quá số lần cho phép" });
            }
            model.IP = IP;
            _unitOfWork.ContactRepository.Insert(model);
            _unitOfWork.Save();
            var subject = "Email liên hệ từ website: " + Request.Url?.Host;
            var body = $"<p>Tên người liên hệ: {model.FullName},</p>" +
                        $"<p>Email liên hệ: {model.Email},</p>" +
                        $"<p>Số điện thoại: {model.Mobile},</p>" +
                        $"<p>Nội dung:{model.Body}</p>" +
                        $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";

            Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, ConfigSite.Email, Email, Email, Password, ConfigSite.Title));
            return Json(new { status = true, msg = "Gửi liên hệ thành công.\nChúng tôi sẽ liên lạc lại với bạn sớm nhất có thể." });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SubcribeForm(string email)
        {
            var isEmail = new EmailAddressAttribute().IsValid(email);
            if (!isEmail || string.IsNullOrEmpty(email))
            {
                return Json(new { status = false, msg = "Email không hợp lệ, vui lòng thử lại!" });
            }
            var IP = HttpContext.Request.UserHostAddress;
            if (IP == null)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            DateTime currentDate = DateTime.Now.Date;
            var count = _unitOfWork.SubcribeRepository.GetQuery(a => a.IP == IP && DbFunctions.TruncateTime(a.CreateDate) == currentDate).Count();
            if (count > 5)
            {
                return Json(new { status = false, msg = "Vượt quá số lần cho phép" });
            }
            Subcribe model = new Subcribe { Email = email, IP = IP };
            _unitOfWork.SubcribeRepository.Insert(model);
            _unitOfWork.Save();
            return Json(new { status = true, msg = "Đăng ký nhân bạn tin thành công!" });
        }
        #region Products
        [Route("san-pham")]
        public ActionResult AllProducts(int? page, string sort)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.CreateDate));
            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Sort = sort,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };
            return View(model);
        }
        public PartialViewResult GetProduct(int? page, string sort)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.CreateDate));
            switch (sort)
            {
                case "date-asc":
                    products = products.OrderBy(a => a.CreateDate);
                    break;
                case "price-asc":
                    products = products.OrderBy(a => a.PriceSale ?? a.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(a => a.PriceSale ?? a.Price);
                    break;
                default:
                    products = products.OrderByDescending(a => a.CreateDate);
                    break;
            }
            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Sort = sort,
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
            };
            return PartialView(model);
        }
        [Route("{url:regex(^(?!.*(vcms|uploader|article|banner|contact|productvcms|user)).*$)}", Order = 2)]
        public ActionResult ProductCategory(string url, int? page, string sort)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var category = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.CategoryActive && a.Url == url).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.ProductCategoryId == category.Id || a.ProductCategory.ParentId == category.Id,
                o => o.OrderByDescending(a => a.CreateDate));
            switch (sort)
            {
                case "date-desc":
                    products = products.OrderByDescending(a => a.CreateDate);
                    break;
                case "date-asc":
                    products = products.OrderBy(a => a.CreateDate);
                    break;
                case "price-asc":
                    products = products.OrderBy(a => a.PriceSale ?? a.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(a => a.PriceSale ?? a.Price);
                    break;
                default:
                    products = products.OrderBy(a => a.Sort);
                    break;
            }
            var model = new CategoryProductViewModel
            {
                Sort = sort,
                Category = category,
                Products = products.ToPagedList(pageNumber, pageSize),
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
                Url = url,
            };
            return View(model);
        }
        public PartialViewResult GetProductCategory(string url, int? page, string sort)
        {
            var pageNumber = page ?? 1;
            var pageSize = 12;
            var category = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.CategoryActive && a.Url == url).FirstOrDefault();
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.ProductCategoryId == category.Id || a.ProductCategory.ParentId == category.Id,
                o => o.OrderByDescending(a => a.CreateDate));
            switch (sort)
            {
                case "date-desc":
                    products = products.OrderByDescending(a => a.CreateDate);
                    break;
                case "date-asc":
                    products = products.OrderBy(a => a.CreateDate);
                    break;
                case "price-asc":
                    products = products.OrderBy(a => a.PriceSale ?? a.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(a => a.PriceSale ?? a.Price);
                    break;
                default:
                    products = products.OrderBy(a => a.Sort);
                    break;
            }
            var model = new CategoryProductViewModel
            {
                Category = category,
                Products = products.ToPagedList(pageNumber, pageSize),
                BeginCount = (pageNumber - 1) * pageSize + 1,
                EndCount = pageNumber * pageSize,
                Url = url,
                Sort = sort,
            };
            return PartialView(model);
        }
        public PartialViewResult MenuLeftProduct()
        {
            var cats = ProductCategories().Where(a => a.ShowCategory);
            return PartialView(cats);
        }
        [Route("tim-kiem")]
        public ActionResult SearchProduct(int? page, string keywords)
        {
            var pageNumber = page ?? 1;
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Name.ToLower().Contains(keywords.ToLower()), o => o.OrderByDescending(a => a.CreateDate));
            var model = new SearchViewModel
            {
                Products = products.ToPagedList(pageNumber, 12),
                Keywords = keywords
            };
            return View(model);
        }

        [Route("{url}.html", Order = 1)]
        public ActionResult ProductDetail(string url)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active &&
            p.Id != product.Id && p.Active &&
            (p.ProductCategoryId == product.ProductCategoryId || p.ProductCategory.ParentId == product.ProductCategoryId),
                    o => o.OrderByDescending(p => Guid.NewGuid()), 6);
            var model = new ProductDetailViewModel
            {
                Product = product,
                Products = products,
                Banners = _unitOfWork.BannerRepository.GetQuery(a => a.Active && a.GroupId == 5, o => o.OrderBy(a => a.Sort))
            };
            return View(model);
        }
        #endregion
        [Route("lien-he")]
        public ActionResult Contact()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}