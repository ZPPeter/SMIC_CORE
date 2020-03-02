using System;
using Abp.Domain.Entities;

namespace SMIC.SDIM
{
    public class STATS : Entity<long>
    {
        public int y { get; set; }
        public int m { get; set; }
        public int count { get; set; }
        public int bm { get; set; }
    }

    public class COUNT : Entity<long>
    {
        public int count { get; set; }

    }


}
