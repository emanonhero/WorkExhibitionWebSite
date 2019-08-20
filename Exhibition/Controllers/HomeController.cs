using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Exhibition.Models;
using System.IO;

namespace Exhibition.Controllers
{
    public class HomeController : Controller
    {
        private WorkContext _workContext;
        public HomeController(WorkContext workContext)
        {
            _workContext = workContext;
        }
        private List<WorkItem> GetWvmList()
        {
            //_workContext.works.Add(new WorkItem() { wName = "t1", Discribe = "dis1", proj = new Project() { pName = "p1", Discribe = "d1" }, imgs = new List<ImgOrVideo>() { new ImgOrVideo() { Src = "~/images/banner1.svg", Discribe = "img1" }, new ImgOrVideo() { Src = "~/images/banner1.svg", Discribe = "img2" }, new ImgOrVideo() { Src = "~/images/banner1.svg", Discribe = "img3" } } });
            //_workContext.works.Add(new WorkItem() { wName = "wt2", Discribe = "dis2", proj = new Project() { pName = "p2", Discribe = "d2" }, imgs = new List<ImgOrVideo>() { new ImgOrVideo() { Src = "~/images/banner1.svg", Discribe = "img5" }, new ImgOrVideo() { Src = "~/images/banner1.svg", Discribe = "img6" }, new ImgOrVideo() { Src = "~/images/banner1.svg", Discribe = "img7" } } });
            //_workContext.SaveChanges();
            var wvmList = _workContext.works.ToList();
            var webPath = Path.Combine(Environment.CurrentDirectory, @"wwwroot\works");
            //DirectoryInfo fdir = new DirectoryInfo(webPath);
            //FileInfo[] files = fdir.GetFiles();
            //string fileName;
            //foreach (FileInfo f in files) //显示当前目录所有文件   
            //{
            //    fileName = f.Name.ToLower();
            //    //if (fileName.EndsWith(".jpg") || fileName.EndsWith(".png") || fileName.EndsWith(".gif") || fileName.EndsWith(".mp4"))
            //    //{
            //    //    list.Add(fileName);
            //    //}
            //}

            //string[] files = Directory.GetFiles(webPath, "*.json");
            //foreach (string file in files)
            //{
            //    Console.WriteLine(file);
            //}

            return wvmList;
        }
        public IActionResult Index()
        {
            if (_workContext.works.Count() == 0) return RedirectToAction("Project");
            return RedirectToAction("Work", "Home", new { id = new Random().Next(1, _workContext.works.Count()+1) });
        }
        [HttpGet("[action]/{id}")]
        public IActionResult Work(int id)
        {
            var item = _workContext.works.Find(id);
            if (item == null)
            {
                return null;
            }
            //显式加载
            _workContext.Entry(item).Collection(w => w.imgs).Load();//加载集合使用Collection方法
            _workContext.Entry(item).Reference(w => w.proj).Load();//加载单个实体使用Reference方法
            //item.proj = _workContext.projects.Find(item.projId);
            //var query = from img in _workContext.imgs
            //            where img.wId == id
            //            select img;
            //item.imgs = query.ToList();
            item.imgs.ForEach(p => p.Src = p.Src.Replace("~", ".."));
            return View(item);
        }
        public IActionResult NewWork()
        {
            ViewData["Message"] = "不能上传与项目直接相关的作品，至少上传一张一片或者视频至作品集。";
            ViewData["Projects"] = _workContext.projects.ToList();
            return View();
        }

        public IActionResult Project()
        {
            ViewData["Message"] = "新增项目信息。";

            return View(_workContext.projects.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
