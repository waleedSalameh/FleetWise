using FleetWise.DAL.Entities.Containers.ExportAqaba;
using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using FleetWise.DAL.Entities.Containers.InternationalExport;
using FleetWise.DAL.Entities.Containers.InternationalIncoming;

namespace FleetWise.DAL.Entities.Customers
{
    public class Customer
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<IncomingAqabaBill> IncomingAqabaBills { get; set; } = new List<IncomingAqabaBill>();

        public ICollection<ExportAqabaBooking> ExportAqabaBookings { get; set; } = new List<ExportAqabaBooking>();

        public ICollection<InternationalIncoming> InternationalIncomings { get; set; } = new List<InternationalIncoming>();
        public ICollection<InternationalExport> InternationalExports { get; set; } = new List<InternationalExport>();
    }
}
