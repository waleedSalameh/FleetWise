using FleetWise.BLL.DTOs.Customers;
using FleetWise.BLL.Interface.Customers;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomer _customerService;

        public CustomersController(ICustomer customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _customerService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null)
                return NotFound(new { message = "Customer not found." });

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
        {
            try
            {
                var customer = await _customerService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
        {
            try
            {
                bool updated = await _customerService.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new { message = "Customer not found." });

                return NoContent();
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _customerService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Customer not found." });

                return NoContent();
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }
    }
}
