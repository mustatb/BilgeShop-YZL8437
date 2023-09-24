using BilgeShop.Business.Services;
using BilgeShop.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BilgeShop.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        public IActionResult Detail(int id)
        {
           var productDetailDto = _productService.GetProductDetailById(id);

            var viewModel = new ProductDetailViewModel()
            {
                ProductId = productDetailDto.ProductId,
                ProductName = productDetailDto.ProductName,
                UnitInStock = productDetailDto.UnitInStock,
                UnitPrice = productDetailDto.UnitPrice,
                ImagePath = productDetailDto.ImagePath,
                Description = productDetailDto.Description,
                CateogoryId = productDetailDto.CateogoryId,
                CategoryName = productDetailDto.CategoryName,
            };

            return View(viewModel);
        }
    }
}
