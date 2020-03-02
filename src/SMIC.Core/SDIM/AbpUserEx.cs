using Abp.Domain.Entities;
using System;

namespace SMIC.SDIM
{    public class AbpUsersEx : Entity<long>
    {
        public DateTime ReadLastNoticeTime { get; set; } // 最近一次读取消息时间
        //public int LogedTimes{ get; set; } //登录次数
        public string ClientType { get; set; }
    }
}
