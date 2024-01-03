using CookHouse.DAL;
using CookHouse.Models;
using CookHouse.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CookHouse.Controllers
{
    [Authorize]
    public class ProductVcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private IEnumerable<ProductCategory> ProductCategories =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private SelectList ParentProductCategoryList => new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");

        #region ProductCategory
        [ChildActionOnly]
        public ActionResult ListCategory()
        {
            var allcats = _unitOfWork.ProductCategoryRepository.Get(orderBy: q => q.OrderBy(a => a.CategorySort));
            return PartialView(allcats);
        }
        public ActionResult ProductCategory(string result = "")
        {
            ViewBag.ArticleCat = result;

            var model = new InsertProductCatViewModel
            {
                RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "Name"),
                ProductCategory = new ProductCategory { CategorySort = 1 },
            };

            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductCategory(InsertProductCatViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "ProductCategory.Image")
                    {
                        model.ProductCategory.Image = imgFile;
                    }
                }
                model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(null, model.ProductCategory.Url ?? model.ProductCategory.CategoryName);
                model.ProductCategory.ParentId = model.CategoryId ?? model.ParentId;
                _unitOfWork.ProductCategoryRepository.Insert(model.ProductCategory);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == model.ProductCategory.Url).Count();
                if (count > 1)
                {
                    model.ProductCategory.Url += "-" + model.ProductCategory.Id;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ProductCategory", new { result = "success" });
            }
            model.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(model);
        }
        public ActionResult UpdateCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            if (category == null)
            {
                return RedirectToAction("ProductCategory");
            }
            var parentCat = ProductCategories.FirstOrDefault(a => a.Id == category.ParentId);
            var model = new InsertProductCatViewModel
            {
                ParentId = 0,
                CategoryId = 0,
                RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "Name"),
                ProductCategory = category,
            };
            if (parentCat != null)
            {
                model.ParentId = Convert.ToInt32(parentCat.Id);
                if (parentCat.ParentId != null)
                {
                    model.ParentId = Convert.ToInt32(parentCat.ParentId);
                    model.CategoryId = parentCat.Id;
                }
            }
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateCategory(InsertProductCatViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "ProductCategory.Image")
                    {
                        model.ProductCategory.Image = imgFile;
                    }
                }

                var file = Request.Files["ProductCategory.Image"];

                if (file != null && file.ContentLength == 0)
                {
                    model.ProductCategory.Image = fc["CurrentFile2"] == "" ? null : fc["CurrentFile2"];
                }
                model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(null, model.ProductCategory.Url ?? model.ProductCategory.CategoryName);
                model.ProductCategory.ParentId = model.CategoryId ?? model.ParentId;
                _unitOfWork.ProductCategoryRepository.Update(model.ProductCategory);
                _unitOfWork.Save();
                var count = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == model.ProductCategory.Url).Count();
                if (count > 1)
                {
                    model.ProductCategory.Url += "-" + model.ProductCategory.Id;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ProductCategory", new { result = "update" });
            }
            model.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            if (category == null)
            {
                return false;
            }
            _unitOfWork.ProductCategoryRepository.Delete(category);
            _unitOfWork.Save();
            return true;
        }
        public bool UpdateProductCat(int sort = 1, bool active = false, bool home = false, bool menu = false, int productCatId = 0)
        {
            var productCat = _unitOfWork.ProductCategoryRepository.GetById(productCatId);
            if (productCat == null)
            {
                return false;
            }
            productCat.CategorySort = sort;
            productCat.CategoryActive = active;
            productCat.ShowMenu = menu;
            productCat.Home = home;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Product  
        public ActionResult ListProduct(int? page, string name, int? parentId, int catId = 0, int thirdId = 0, string sort = "sort-asc", string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var products = _unitOfWork.ProductRepository.GetQuery().AsNoTracking();

            if (thirdId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == thirdId);
            }
            else if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            else if (parentId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == parentId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(l => l.Name.Contains(name));
            }

            switch (sort)
            {
                case "date-desc":
                    products = products.OrderByDescending(a => a.CreateDate);
                    break;
                case "sort-desc":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "hot":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "date-asc":
                    products = products.OrderBy(a => a.CreateDate);
                    break;
                default:
                    products = products.OrderBy(a => a.Sort);
                    break;
            }
            var model = new ListProductViewModel
            {
                SelectCategories = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                Products = products.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                ParentId = parentId,
                ThirdId = thirdId,
                Name = name,
                Sort = sort
            };
            if (parentId.HasValue)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == parentId), "Id", "CategoryName");
            }
            if (catId > 0)
            {
                model.ThirdCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == catId), "Id", "CategoryName");
            }
            return View(model);
        }
        public ActionResult Product()
        {
            var model = new InsertProductViewModel
            {
                Product = new Product { Sort = 1, Active = true },
                Categories = ProductCategories,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Product(InsertProductViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                if (model.Price != null)
                {
                    model.Product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
                }
                if (model.PriceSale != null)
                {
                    model.Product.PriceSale = Convert.ToDecimal(model.PriceSale.Replace(",", ""));
                }

                model.Product.ListImage = fc["Pictures"];
                model.Product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                model.Product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                _unitOfWork.ProductRepository.Insert(model.Product);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == model.Product.Url).Count();
                if (count > 1)
                {
                    model.Product.Url += "-" + DateTime.Now.Millisecond;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListProduct", new { result = "success" });
            }

            model.Categories = ProductCategories;
            return View(model);
        }
        public ActionResult UpdateProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }

            var model = new InsertProductViewModel
            {
                Product = product,
                Categories = ProductCategories,
                Price = product.Price?.ToString("N0"),
                PriceSale = product.PriceSale?.ToString("N0"),
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateProduct(InsertProductViewModel model, FormCollection fc)
        {
            var product = _unitOfWork.ProductRepository.GetById(model.Product.Id);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }

            if (ModelState.IsValid)
            {
                if (model.Price != null)
                {
                    product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
                }
                else
                {
                    product.Price = null;
                }
                if (model.PriceSale != null)
                {
                    product.PriceSale = Convert.ToDecimal(model.PriceSale.Replace(",", ""));
                }
                else
                {
                    product.PriceSale = null;
                }

                product.ListImage = fc["Pictures"] == "" ? null : fc["Pictures"];
                product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                product.ProductCategoryId = model.CategoryId ?? model.ParentId;
                product.Name = model.Product.Name;
                product.Body = model.Product.Body;
                product.Active = model.Product.Active;
                product.Home = model.Product.Home;
                product.Hot = model.Product.Hot;
                product.TitleMeta = model.Product.TitleMeta;
                product.DescriptionMeta = model.Product.DescriptionMeta;
                product.Sort = model.Product.Sort;
                product.Description = model.Product.Description;
                product.StockOff = model.Product.StockOff;
                product.Code = model.Product.Code;

                _unitOfWork.Save();

                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == product.Url).Count();
                if (count > 1)
                {
                    product.Url += "-" + DateTime.Now.Millisecond;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListProduct", new { result = "update" });
            }
            model.Categories = ProductCategories;
            return View(model);
        }
        [HttpPost]
        public bool DeleteProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            _unitOfWork.ProductRepository.Delete(product);
            _unitOfWork.Save();
            return true;
        }
        public bool QuickUpdate(int sort = 1, int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            product.Sort = sort;

            _unitOfWork.Save();
            return true;
        }
        public PartialViewResult GetPropertyByGroup(int groupId)
        {
            var groupProperty = _unitOfWork.ProductCategoryRepository.GetById(groupId);
            return PartialView(groupProperty);
        }
        #endregion

        public JsonResult GetProductCategory(int? parentId)
        {
            var categories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(categories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChildCategory(int parentId = 0)
        {
            var childCategories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(childCategories.Select(a => new { a.Id, a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetThirdProductCategory(int? childId)
        {
            var thirdCategories = ProductCategories.Where(a => a.ParentId == childId);
            return Json(thirdCategories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}