using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Containers
{
    public class DeliveryLocation
    {
        public int DeliveryLocationId { get; set; }

        public string Name { get; set; } = string.Empty;
        public ICollection<IncomingAqabaContainer> Containers { get; set; }
    = new List<IncomingAqabaContainer>();
        public bool IsDeleted { get; set; }
    }
}
