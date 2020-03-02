using System;
using Abp.Domain.Entities;

namespace SMIC.SDIM
{
    public class JBCS : Entity<long>
    {
        public double BCJDA { get; set; }
        public double BCJDB { get; set; }
        public double CJJD { get; set; }
        public double BCFW { get; set; }
        public int Axles { get; set; }
    }
}
