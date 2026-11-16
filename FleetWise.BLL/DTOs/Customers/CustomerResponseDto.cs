namespace FleetWise.BLL.DTOs.Customers
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
    }
}
