using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.Data.Entities;
using BilgeShop.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Business.Managers
{
	public class CategoryManager : ICategoryService
	{
		private readonly IRepository<CategoryEntity> _categoryRepository;
        public CategoryManager(IRepository<CategoryEntity>categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public bool AddCategory(AddCategoryDto addCategoryDto)
		{
			var hasCategory = _categoryRepository.GetAll(x=> x.Name.ToLower() == addCategoryDto.Name).ToList();

			if(hasCategory.Any()) // hasCategory != 0 demek (yeni .Net 6.00)
			{
              return false;
			}
			var categoryEntity = new CategoryEntity()
			{
				Name = addCategoryDto.Name,
				Description = addCategoryDto.Description
			};
			_categoryRepository.Add(categoryEntity);

			return true;
		}

		public void DeleteCategory(int Id)
		{
			_categoryRepository.Delete(Id);
			// TODO : CategoryId ile eşleşen bütün ürünler de SoftDelete edilecek
		}

		public void EditCategory(EditCategoryDto editCategoryDto)
		{
			var categoryEntity = _categoryRepository.GetById(editCategoryDto.Id);

			categoryEntity.Name = editCategoryDto.Name;
			categoryEntity.Description = editCategoryDto.Description;

			_categoryRepository.Update(categoryEntity);
		}

		public List<ListCategoryDto> GetCategories()
		{
			var categoryEntities = _categoryRepository.GetAll().OrderBy(x => x.Name);

			var categoryDtoList = categoryEntities.Select(x => new ListCategoryDto
			{
				Id = x.Id,
				Name = x.Name,
				Description = x.Description,
			}).ToList();
			//Her bir entity için ( x ) 1 adet ListCategoryDto nesnesi newleyip verileri aktarıyorum.
			// Özetle bir tür listeden diğer tüm listeye çeviriyorum.

			return categoryDtoList;
		}

		public EditCategoryDto GetCategoryById(int Id)
		{
			var categoryEntity = _categoryRepository.GetById(Id);

			var editCategoryDto = new EditCategoryDto()
			{
				Id = categoryEntity.Id,
				Name = categoryEntity.Name,
				Description = categoryEntity.Description,

			};
			return editCategoryDto;
		}
	}
}
