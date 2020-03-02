using System;
using Abp.Domain.Entities;

namespace SMIC.SDIM
{
    public class JDRQ : Entity<long>
    {
        public DateTime jdrq { get; set; }
        public DateTime jwrq { get; set; }
        public int jdzt { get; set; }
    }
}
