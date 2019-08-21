using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Exhibition.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace Exhibition.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private WorkContext _workContext;
        private readonly IHostingEnvironment _hostingEnvironment;

        public WorkController(WorkContext workContext, IHostingEnvironment hostingEnvironment)
        {
            _workContext = workContext;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public ActionResult<string> Get()
        {
            string res = string.Empty;
            //使用Include方法指定要包含在查询结果中的关联数据。关联数据可以是有层级的，可通过链式调用ThenInclude，进一步包含更深级别的关联数据。
            //https://www.cnblogs.com/youring2/p/11186614.html
            foreach (var w in _workContext.works.Include(w => w.imgs).Include(w => w.proj).ToArray())
            {
                res += JsonConvert.SerializeObject(w) + Environment.NewLine;
            }
            return res;
        }
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
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
            return item.ToString();
        }
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            //var files = Request.Form.Files;
            //long size = files.Sum(f => f.Length);
            string webRootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            long size = files.Sum(f => f.Length);
            List<string> localFiles = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string newFileName = GetFileGuidName(formFile.FileName) + GetFileExt(formFile.FileName); //随机生成新的文件名
                    var filePath = Path.Combine(webRootPath, "works", newFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    localFiles.Add(filePath);
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, localFiles });
        }

        private string GetFileExt(string fileName) => Path.GetExtension(fileName).ToLower();
        private string GetFileGuidName(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return fileName.Substring(0, Math.Min(20, fileName.Length)) + "_" + System.Guid.NewGuid().ToString();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadNewWorkItem(List<IFormFile> files, [FromForm]string name, [FromForm]string discribe,[FromForm]string author,[FromForm]int projId, [FromForm]string[] imgDiscribe)
        {
            WorkItem newWork = new WorkItem(name, discribe);
            newWork.imgs = new List<ImgOrVideo>();
            string webRootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            long size = files.Sum(f => f.Length);
            List<string> localFiles = new List<string>();
            if (files.Count != imgDiscribe.Length||files.Count==0) return Ok(new { files, imgDiscribe });
            for (int i = 0; i < files.Count; i++)
            {
                IFormFile formFile = files[i];
                if (formFile.Length > 0)
                {
                    string newFileName = GetFileGuidName(formFile.FileName) + GetFileExt(formFile.FileName); //随机生成新的文件名
                    var filePath = Path.Combine(webRootPath, "works", newFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    newWork.imgs.Add(new ImgOrVideo() { Src = "~/works/" + newFileName ,Discribe=imgDiscribe[i]});
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            //newWork.proj = new Project() { pName = "pt3", Discribe = "detail1" };//proj不能为空，否则报错FOREIGN KEY constraint failed
            newWork.projId = projId;
            newWork.author = author;
            newWork.editTime = DateTime.Now.ToString("s");
            //Console.WriteLine(DateTime.Parse(newWork.editTime));
            _workContext.works.Add(newWork);
            _workContext.SaveChanges();
            return Ok(new { count = files.Count, size, newWork });
        }

        [HttpPost("[action]")]
        public IActionResult NewProject([FromForm]string pName, [FromForm]string Discribe)
        {
            Project newPro = new Project() { pName = pName, Discribe = Discribe };
            newPro = _workContext.projects.Add(newPro).Entity;
            _workContext.SaveChanges();
            //return Ok(new { newPro });
            return RedirectToAction("Project", "Home");
        }        
        
    }
}