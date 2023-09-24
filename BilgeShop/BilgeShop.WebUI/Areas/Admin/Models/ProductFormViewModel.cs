using System.ComponentModel.DataAnnotations;

namespace BilgeShop.WebUI.Areas.Admin.Models
{
	public class ProductFormViewModel
	{
		public int Id { get; set; }

		[Display(Name = "Ürün Adı")]
		[Required(ErrorMessage = "Ürün adı alanını doldurmak zorunludur.")]
		public string Name { get; set; }

		[Display(Name = "Açıklama")]
		[MaxLength(1000)]

		public string? Description { get; set; }

		[Display(Name = "Ürün Fiyatı")]
		[Required(ErrorMessage = "Ürün fiyatı alanını doldurmak zorunludur.")]
		public decimal? UnitPrice { get; set; }

        [Display(Name = "Ürün Stok Durumu")]
        public int UnitStock { get; set; }

		[Display(Name="Kategori")]
		[Required(ErrorMessage = "Bir kategori seçmek zorunludur.")]
        public int CategoryId { get; set; }

		[Display(Name="Ürün Girilmedi")]
        public IFormFile? File { get; set; }

        // TODO : Görsel taşınacak.

    }
}
