using System;
using System.Collections.Generic;
using Abp.Domain.Entities;

namespace SMIC.SDIM
{
    public class HomeInfo : Entity<int>
    {
        public string User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }        
        public DateTime? CreationTime { get; set; }

        public HomeInfo()            
        {
            User = "admin"; // 编辑权限,目前未用
            CreationTime = DateTime.Now;
        }

        public HomeInfo(string description, DateTime? creationTime, string title = null) : this()
        {
            Title = title;
            Description = description;
            CreationTime = creationTime;
        }
    }

}
