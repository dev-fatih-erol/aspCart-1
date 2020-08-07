using System.Linq;
using aspCart.Core.Interface.Services.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace aspCart.Web.ViewComponents
{
    [ViewComponent(Name = "PopulerCategory")]
    public class PopulerCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public PopulerCategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IViewComponentResult Invoke()
        {
            return View(
                _categoryService.GetAllCategoriesWithoutParent().Where(x => x.Published)
            );
        }
    }
}