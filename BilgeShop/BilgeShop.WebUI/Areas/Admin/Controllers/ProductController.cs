using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilgeShop.WebUI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ProductController : Controller
	{
		private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment;
		public ProductController(ICategoryService categoryService, IProductService productService, IWebHostEnvironment environment)
		{
			_categoryService = categoryService;
            _productService = productService;
            _environment = environment;
		}
		public IActionResult List()
		{
            var productDtoList = _productService.GetProducts();

            // Select ile bir tür listeden diğer tür listeye çeviriyorum.
            // Her bir elemanı için yeni bir ListProductViewModel açılıp veriler aktarılıyor. 

            var viewModel = productDtoList.Select(x => new ListProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                UnitInStock = x.UnitInStock,
                UnitPrice = x.UnitPrice,
                ImagePath = x.ImagePath
            }).ToList();

			return View(viewModel);
		}
		public IActionResult New()
		{
			ViewBag.Categories = _categoryService.GetCategories();

			return View("Form", new ProductFormViewModel());
		}

        public IActionResult Edit(int id)
        {
            var editProductDto = _productService.GetProductById(id);

            var viewModel = new ProductFormViewModel()
            {
                Id = editProductDto.Id,
                Name = editProductDto.Name,
                CategoryId = editProductDto.CategoryId,
                Description = editProductDto.Description,
                UnitPrice = editProductDto.UnitPrice,
                UnitStock = editProductDto.UnitInStock
            };

            // Eski görseli ekranda görmek istiyorum. O yüzden viewbag ile ilgili view gönderiyorum.
            ViewBag.ImagePath = editProductDto.ImagePath;
            ViewBag.Categories = _categoryService.GetCategories();

            return View("form", viewModel);
        }


        [HttpPost]
        public IActionResult Save(ProductFormViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetCategories();
                return View("Form", formData);
            }
            var newFileName = "";

            if(formData.File is not null) // bir görsel gönderildi ise
            {
                var allowedFileTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/jfif" };
                //izin vereceğim dosya türleri

                var allowedFieExtensions = new string[] { ".jpg", ".jpeg", ".png", ".jfif" };
                // izin vereceğim dosya uzantıları.

                var fileContentType = formData.File.ContentType; // dosyanın içerik tipi

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(formData.File.FileName);//uzantısız dosya ismi

                var fileExtension = Path.GetExtension(formData.File.FileName); // uzantı.

                if(!allowedFileTypes.Contains(fileContentType) || !allowedFieExtensions.Contains(fileExtension))
                {
                    ViewBag.FileError = "Dosya formatı veya içeriği hatalı";

                    ViewBag.Categories = _categoryService.GetCategories();
                    return View("Form", formData);
                }
                newFileName = fileNameWithoutExtension + "-" + Guid.NewGuid() + fileExtension;
                // Aynı isimde iki dosya yüklendiğinde hata vermesin, birbiriyle asla eşleşmeyecek şekilde her dosya adına uniq(eşsiz) bir metin ilavesi yapıyorum.

                var folderPath = Path.Combine("images", "products"); // images/products

                var wwwrootFolderPath = Path.Combine(_environment.WebRootPath, folderPath);
                // ...wwwroot/images/products

                var wwwrootFilePath = Path.Combine(wwwrootFolderPath, newFileName);
                // ...wwwroot/images/products/urunGorseli-12312312.jpg

                Directory.CreateDirectory(wwwrootFolderPath); // Eğer images/products yoksa oluştur.

                using (var filestream = new FileStream(wwwrootFilePath, FileMode.Create))
                {
                    formData.File.CopyTo(filestream);
                }
                // asıl dosyayı kopyaladığım kısım.

                //using içerisinde new'lenen filestream nesnesi scope boyunca yaşar, scope bitimi silinir.
            }

            if (formData.Id == 0) // yeni kayıt
            {
                var addProductDto = new AddProductDto()
                {
                    Name = formData.Name.Trim(),
                    UnitPrice = formData.UnitPrice,
                    UnitStock = formData.UnitStock,
                    CategoryId = formData.CategoryId,
                    Description = formData.Description,
                    ImagePath = newFileName

                };

                // Description null olursa trim işlemi sırasında uygulama exception verir. O nedenle trim yapmak istiyorsak aşağıdaki kontrolü yapmalıyız.
                if (formData.Description is not null)
                {
                    addProductDto.Description = formData.Description.Trim();
                }

                var result= _productService.AddProduct(addProductDto);

                
                if (result)
                {
                    RedirectToAction("List");
                }
                else
                {
                    ViewBag.ErrorMessage = "Bu işlemde bir Ürün zaten mevcut";
					ViewBag.Categories = _categoryService.GetCategories();
					return View("Form", formData);
                }
            }
            else // Güncelleme
            {
                var editProductDto = new EditProductDto()
                {
                    Id = formData.Id,
                    Name = formData.Name,
                    UnitPrice = formData.UnitPrice,
                    UnitInStock = formData.UnitStock,
                    Description = formData.Description,
                    CategoryId = formData.CategoryId,
                };
                if(formData.File is not null)
                {
                    editProductDto.ImagePath = newFileName;
                }
                // Bu kontrolü hem controller hem business katmanında yapacağım. Yeni bir dosya seçilmediyse    yeni null gönderildiyse Db'de olan görselin üzerine yazılmasını istemiyorum.

                _productService.EditProduct(editProductDto);
            }

            return RedirectToAction("List");
        }

        public IActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);

            return RedirectToAction("List");
        }
    }
}
