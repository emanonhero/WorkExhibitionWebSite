using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
//Sqlite清空表并重置自增ID。
//DELETE FROM 'TblName';
//DELETE FROM sqlite_sequence WHERE name = 'TblName';
namespace Exhibition.Models
{
    public class WorkContext : DbContext
    {
        public DbSet<WorkItem> works { get; set; }
        public DbSet<ImgOrVideo> imgs { get; set; }
        public DbSet<Project> projects { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=WorkContext.db");
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<WorkItem>(e =>
        //    {
        //        e.HasIndex(x => x.Discribe).IsFullText(); // 添加全文索引
        //    });
        //}
    }
    public class WorkItem
    {
        [Key]
        public int wId { get; set; }
        public List<ImgOrVideo> imgs { get; set; }//"~/images/banner1.svg"
        public string wName { get; set; }
        public string author { get; set; }
        [ForeignKey("proj")]
        public int projId { get; set; }
        public Project proj { get; set; }
        public string Discribe { get; set; }
        //[Timestamp]
        public string editTime { get; set; }//sqlite DateTime有读取错误bug
        public WorkItem(string wName, string Discribe)
        {
            this.wName = wName;
            this.Discribe = Discribe;
        }
        public override string ToString()
        {
            //var item = _workContext.works.Find(id);
            //if (item == null)
            //{
            //    return null;
            //}
            //item.proj = _workContext.projects.Find(item.projId);
            //var query = from img in _workContext.imgs
            //            where img.wId == id
            //            select img;
            //item.imgs = query.ToList();
            return JsonConvert.SerializeObject(this);
        }
    }
    public class ImgOrVideo
    {
        [Key]
        public int Id { get; set; }
        public string Src { get; set; }
        public string Discribe { get; set; }
        [ForeignKey("WorkItem")]
        public int wId { get; set; }
        //public virtual WorkItem work { get; set; }//错误：会出现循环调用
    }
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string pName { get; set; }
        public string Discribe { get; set; }
    }
}
