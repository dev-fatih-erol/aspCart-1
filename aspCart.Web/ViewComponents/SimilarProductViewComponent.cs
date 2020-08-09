﻿using System;
using System.Collections.Generic;
using System.Linq;
using aspCart.Core.Domain.Catalog;
using aspCart.Core.Interface.Services.Catalog;
using aspCart.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace aspCart.Web.ViewComponents
{
    [ViewComponent(Name = "SimilarProduct")]
    public class SimilarProductViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public SimilarProductViewComponent(IProductService productService,
            IReviewService reviewService,
            IMapper mapper)
        {
            _productService = productService;
            _reviewService = reviewService;
            _mapper = mapper;
        }

        public IViewComponentResult Invoke()
        {
            var productEntities = _productService.GetAllProducts()
                .Where(x => x.Published == true)
                .OrderBy(x => Guid.NewGuid()).Take(12);

            var productList = new List<ProductModel>();

            foreach (var product in productEntities)
            {
                var productModel = _mapper.Map<Product, ProductModel>(product);

                productModel.Categories = product.Categories
                    .Select(x => new CategoryModel { Name = x.Category.Name, SeoUrl = x.Category.SeoUrl })
                    .ToList();

                // get main image
                if (product.Images.Count > 0)
                {
                    productModel.MainImage = product.Images
                        .OrderBy(x => x.SortOrder)
                        .ThenBy(x => x.Position)
                        .FirstOrDefault()
                        .Image
                        .FileName;
                }

                // check for discount
                if (product.SpecialPriceEndDate != null && product.SpecialPriceEndDate >= DateTime.Now)
                {
                    productModel.OldPrice = product.Price;
                    productModel.Price = product.SpecialPrice ?? productModel.OldPrice;
                }

                //get product rating
                var reviews = _reviewService.GetReviewsByProductId(productModel.Id);
                if (reviews != null && reviews.Count > 0)
                {
                    productModel.Rating = reviews.Sum(x => x.Rating);
                    productModel.Rating /= reviews.Count;
                }

                productList.Add(productModel);
            }

            // silinecek
            var p = productList.First();
            productList.Add(p);
            return View(productList);
        }
    }
}