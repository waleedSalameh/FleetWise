using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.ShippingLines
{
    public class CreateShippingLineDto
    {
        [Required(ErrorMessage = "Please enter the shipping line name.")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the shipping line code.")]
        [MaxLength(30)]
        public string Code { get; set; } = string.Empty;
    }
}
