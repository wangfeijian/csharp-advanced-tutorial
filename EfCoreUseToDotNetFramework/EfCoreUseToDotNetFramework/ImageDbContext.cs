using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfCoreUseToDotNetFramework
{
    // 1、安装下面这些包，选.net standard 2.0的版本
    // Microsoft.EntityFrameworkCore：核心包，不多说
    // Microsoft.EntityFrameworkCore.Tool：支持 PS 命令的 Code First 工具包
    // Microsoft.EntityFrameworkCore.Design：Code First 必备包
    // Microsoft.EntityFrameworkCore.Sqlite：Sqlite 驱动
    public class ImageDbContext : DbContext
    {
        public DbSet<ImageInfo> ImageData { get; set; }

        // Add-Migration
        // Update-Database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Image.db");
        }
    }

    public class ImageInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatTime { get; set; }
        public string SN { get; set; }
        public string VisionStep { get; set; }
        public string Data { get; set; }
    }
}
