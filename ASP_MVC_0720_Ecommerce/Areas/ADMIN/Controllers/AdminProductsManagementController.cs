using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models;
using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Services;
using ASP_MVC_0720_Ecommerce.Areas.ADMIN.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Controllers
{
    public class AdminProductsManagementController : Controller
    {
        ProductsManagementService productsmanagementService = new ProductsManagementService();

        // GET: ADMIN/AdminProductsManagement
        [Authorize(Roles = "Admin")]
        public ActionResult Product(int? page)
        {
            List<AdminProducts> ProductList = new List<AdminProducts>();

            ProductList = productsmanagementService.GetAllProducts();

            var pageNumber = page ?? 1;
            var onePageOfProducts = ProductList.ToPagedList(pageNumber, 5);

            return View(onePageOfProducts);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DelProduct(string Product_No)
        {
            productsmanagementService.DelProduct(Product_No);
            TempData["msg"] = "刪除成功！";
            return RedirectToAction("Product", "AdminProductsManagement");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditProduct(string Product_No)
        {
            AdminProducts ProductData = new AdminProducts();

            ProductData = productsmanagementService.GetOneProduct(Product_No);

            List<SelectListItem> Data = new List<SelectListItem>();
            Data.Add(new SelectListItem { Text = "是", Value = "1" });
            Data.Add(new SelectListItem { Text = "否", Value = "0" });

            ViewBag.IsOnList = Data;
            return View(ProductData);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditProduct(AdminProducts EditProduct)
        {
            if (EditProduct.ProductImage != null)
            {
                if (productsmanagementService.CheckImg(EditProduct.ProductImage.ContentType))
                {
                    /*實際上傳路徑*/
                    string path = Server.MapPath(@"~/Upload/Products/");
                    path += EditProduct.Product_Id.ToString();

                    //取得檔名
                    string filename = Path.GetFileName(EditProduct.ProductImage.FileName);

                    if (Directory.Exists(path))
                    {
                        /*刪除資料夾內檔案重新上傳*/
                        string[] files = Directory.GetFiles(path);

                        for (int i = 0; i < 1; i++)
                        {
                            if (files.Length > 0)
                            {
                                System.IO.File.Delete(files[i]);
                            }
                        }

                        string url = Path.Combine(path, filename);

                        EditProduct.ProductImage.SaveAs(url);
                        EditProduct.Image = filename;
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                        string url = Path.Combine(path, filename);

                        EditProduct.ProductImage.SaveAs(url);
                        EditProduct.Image = filename;
                    }

                }
            }
            productsmanagementService.EditProduct(EditProduct);

            TempData["msg"] = "修改成功！";

            return RedirectToAction("Product", "AdminProductsManagement");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            List<SelectListItem> Data = new List<SelectListItem>();
            Data.Add(new SelectListItem { Text = "是", Value = "1" });
            Data.Add(new SelectListItem { Text = "否", Value = "0", Selected = true });

            ViewBag.IsOnList = Data;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(AdminProducts EditProduct)
        {
            List<SelectListItem> Data = new List<SelectListItem>();
            Data.Add(new SelectListItem { Text = "是", Value = "1" });
            Data.Add(new SelectListItem { Text = "否", Value = "0", Selected = true });

            ViewBag.IsOnList = Data;
            if (ModelState.IsValid)
            {
                if (EditProduct.ProductImage != null)
                {
                    if (productsmanagementService.CheckImg(EditProduct.ProductImage.ContentType))
                    {
                        int newId = productsmanagementService.GetNewId();
                        /*實際上傳路徑*/
                        string path = Server.MapPath(@"~/Upload/Products/");
                        path += newId;

                        //取得檔名
                        string filename = Path.GetFileName(EditProduct.ProductImage.FileName);

                        Directory.CreateDirectory(path);
                        string url = Path.Combine(path, filename);

                        EditProduct.ProductImage.SaveAs(url);
                        EditProduct.Image = filename;
                    }
                }
                productsmanagementService.CreateProduct(EditProduct);
                TempData["msg"] = "新增成功！";
                return RedirectToAction("Product", "AdminProductsManagement");
            }
            return View();

        }
    }
}