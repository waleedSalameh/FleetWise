using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Containers
{
    public class ShippingLine
    {
        public int ShippingLineId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
        public ICollection<IncomingAqabaContainer> Containers { get; set; }
    = new List<IncomingAqabaContainer>();
        public bool IsDeleted { get; set; }
    }
}
