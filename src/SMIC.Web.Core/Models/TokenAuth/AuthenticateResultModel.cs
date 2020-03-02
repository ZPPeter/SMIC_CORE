using System;

namespace SMIC.Models.TokenAuth
{
    public class AuthenticateResultModel
    {
        public string AccessToken { get; set; }

        public string EncryptedAccessToken { get; set; }

        public int ExpireInSeconds { get; set; }

        public long UserId { get; set; }
    }
    public class AuthenticateResultModelEx
    {
        public string AccessToken { get; set; }

        public string EncryptedAccessToken { get; set; }

        public int ExpireInSeconds { get; set; }

        public long UserId { get; set; }

        public string SurName { get; set; }

        public string[] Roles { get; set; }//NormalizedName ["一般用户", "1000", "1030"]

        public string[] RoleNames { get; set; } //DisplayName ["一般用户", "全站仪", "GPS接收机"]

        public DateTime? LastReadNoticeTime { get; set; }
                
    }
}
