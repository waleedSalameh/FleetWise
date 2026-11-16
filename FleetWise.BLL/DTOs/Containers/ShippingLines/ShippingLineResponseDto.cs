using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.ShippingLines
{
    public class ShippingLineResponseDto
    {
        public int ShippingLineId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }
}
