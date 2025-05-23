using LPRStoresAPI.DTOs;
using LPRStoresAPI.Models; // For Product model if mapping manually
using LPRStoresAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // For ArgumentException
using Microsoft.AspNetCore.Http; // For StatusCodes

namespace LPRStoresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        [AllowAnonymous] // Products can be viewed by anyone
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            var productDtos = products.Select(p => new ProductDto { 
                ProductId = p.ProductId, 
                Name = p.Name, 
                Description = p.Description, 
                Price = p.Price, 
                StockQuantity = p.StockQuantity, 
                ReorderLevel = p.ReorderLevel 
            }).ToList();
            return Ok(productDtos);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(new ProductDto { 
                ProductId = product.ProductId, 
                Name = product.Name, 
                Description = product.Description, 
                Price = product.Price, 
                StockQuantity = product.StockQuantity, 
                ReorderLevel = product.ReorderLevel 
            });
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/products
        ///     {
        ///        "name": "Heavy Duty Hammer",
        ///        "description": "A very sturdy hammer for tough jobs.",
        ///        "price": 25.99,
        ///        "stockQuantity": 50,
        ///        "reorderLevel": 10
        ///     }
        ///
        /// </remarks>
        /// <param name="createProductDto">The product information to create.</param>
        /// <returns>The newly created product.</returns>
        /// <response code="201">Returns the newly created product.</response>
        /// <response code="400">If the product data is invalid or product name already exists.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not authorized (Admin/Manager role required).</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")] 
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var product = new Product { 
                Name = createProductDto.Name, 
                Description = createProductDto.Description, 
                Price = createProductDto.Price, 
                StockQuantity = createProductDto.StockQuantity, 
                ReorderLevel = createProductDto.ReorderLevel 
            };
            try
            {
                 var createdProduct = await _productService.CreateProductAsync(product);
                 var productDto = new ProductDto { 
                     ProductId = createdProduct.ProductId, 
                     Name = createdProduct.Name, 
                     Description = createdProduct.Description,
                     Price = createdProduct.Price, 
                     StockQuantity = createdProduct.StockQuantity,
                     ReorderLevel = createdProduct.ReorderLevel
                 };
                 return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, productDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var productToUpdate = await _productService.GetProductByIdAsync(id);
            if (productToUpdate == null) return NotFound();
            
            productToUpdate.Name = updateProductDto.Name;
            productToUpdate.Description = updateProductDto.Description;
            productToUpdate.Price = updateProductDto.Price;
            productToUpdate.StockQuantity = updateProductDto.StockQuantity;
            productToUpdate.ReorderLevel = updateProductDto.ReorderLevel;
            
            try
            {
                var success = await _productService.UpdateProductAsync(productToUpdate);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // GET: api/products/lowstock
        [HttpGet("lowstock")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts()
        {
            var products = await _productService.GetLowStockProductsAsync();
            var productDtos = products.Select(p => new ProductDto { 
                ProductId = p.ProductId, 
                Name = p.Name, 
                Description = p.Description, 
                StockQuantity = p.StockQuantity, 
                ReorderLevel = p.ReorderLevel, 
                Price = p.Price 
            }).ToList();
            return Ok(productDtos);
        }
    }
}
