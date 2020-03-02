namespace SMIC
{
    public class SMICConsts
    {
        public const string LocalizationSourceName = "SMIC";

        public const string ConnectionStringName = "Default";
        public const string SDIMConnectionStringName = "SDIM";

        public const bool MultiTenancyEnabled = false;
        
        /// <summary>
        /// 邮件地址最大长度
        /// </summary>
        public const int MaxEmailAddressLength = 200;

        /// <summary>
        /// 名字最大长度
        /// </summary>
        public const int MaxNameLength = 50;
        public const int MaxAddressLength = 200;

        public static class SchemaNames
        {
            public const string Basic = "Basic";

            public const string ABP = "ABP";

            public const string CMS = "CMS";
        }

        /// <summary>
        /// 实体长度单位
        /// </summary>
        public static class EntityLengthNames
        {
            public const int Length8 = 8;
            public const int Length16 = 16;
            public const int Length32 = 32;
            public const int Length64 = 65;
            public const int Length128 = 128;
            public const int Length256 = 256;
            public const int Length512 = 512;

            public const int Length1024 = 1024;

        }

    }
}
