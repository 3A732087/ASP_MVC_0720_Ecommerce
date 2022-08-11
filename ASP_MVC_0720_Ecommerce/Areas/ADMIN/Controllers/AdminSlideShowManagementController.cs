using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models;
using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Services;
using ASP_MVC_0720_Ecommerce.Areas.ADMIN.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Controllers
{
    public class AdminSlideShowManagementController : Controller
    {
        SlideShowManagementService slideShowmanagementService = new SlideShowManagementService();

        // GET: ADMIN/AdminSlideShowManagement
        [Authorize(Roles = "Admin")]
        public ActionResult SlideShow()
        {
            AllSlideShowViewModel DataList = new AllSlideShowViewModel();
            DataList.SlideShowList = slideShowmanagementService.GetAllSlideShow();
            return View(DataList);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(AdminSlideShow newSlideShow)
        {
            if (ModelState.IsValid)
            {
                if (newSlideShow.SlideShowImg != null)
                {
                    if (slideShowmanagementService.CheckImg(newSlideShow.SlideShowImg.ContentType))
                    {
                        int newId = slideShowmanagementService.GetNewId();
                        /*實際上傳路徑*/
                        string path = Server.MapPath(@"~/Upload/SlideShow/");
                        path += newId;

                        //取得檔名
                        string filename = Path.GetFileName(newSlideShow.SlideShowImg.FileName);

                        Directory.CreateDirectory(path);
                        string url = Path.Combine(path, filename);

                        newSlideShow.SlideShowImg.SaveAs(url);
                        newSlideShow.Img_Name = filename;


                    }
                }
                slideShowmanagementService.CreateSlideShow(newSlideShow);
                TempData["msg"] = "新增成功！";
                return RedirectToAction("SlideShow", "AdminSlideShowManagement");
            }
            else
            {
                ModelState.AddModelError("SlideShowImg", "");
                return View();
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditSlideShow(string Img_Id)
        {
            AdminSlideShow Data = new AdminSlideShow();
            Data = slideShowmanagementService.GetOneSlideShowImg(Img_Id);
            return View(Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditSlideShow(AdminSlideShow EditSlideShow)
        {
            if (ModelState.IsValid)
            {
                if (EditSlideShow.SlideShowImg != null)
                {
                    if (slideShowmanagementService.CheckImg(EditSlideShow.SlideShowImg.ContentType))
                    {
                        /*實際上傳路徑*/
                        string path = Server.MapPath(@"~/Upload/SlideShow/");
                        path += EditSlideShow.Img_Id.ToString();

                        //取得檔名
                        string filename = Path.GetFileName(EditSlideShow.SlideShowImg.FileName);

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

                            EditSlideShow.SlideShowImg.SaveAs(url);
                            EditSlideShow.Img_Name = filename;
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            string url = Path.Combine(path, filename);

                            EditSlideShow.SlideShowImg.SaveAs(url);
                            EditSlideShow.Img_Name = filename;
                        }

                    }
                }
                slideShowmanagementService.EditSlideShow(EditSlideShow);
                TempData["msg"] = "編輯成功！";
                return RedirectToAction("SlideShow", "AdminSlideShowManagement");
            }
            else
            {
                ModelState.AddModelError("SlideShowImg", "");
                return View(EditSlideShow);
            }

        }


        [Authorize(Roles = "Admin")]
        public ActionResult DelSlideShow(string Img_Id)
        {
            /*路徑*/
            string path = Server.MapPath(@"~/Upload/SlideShow/");
            path += Img_Id;
            /*刪除資料夾內檔案重新上傳*/
            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < 1; i++)
            {
                if (files.Length > 0)
                {
                    System.IO.File.Delete(files[i]);
                }
            }
            Directory.Delete(path);

            slideShowmanagementService.DelSlideShowImg(Img_Id);
            TempData["msg"] = "刪除成功！";
            return RedirectToAction("SlideShow", "AdminSlideShowManagement");
        }
    }
}