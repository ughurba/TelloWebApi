﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.ProductDtos.ProductCreateDto
{
    public class ProductCreateDto
    {
        public int Id { get; set; }
     
        public string Title { get; set; }
        public string Description { get; set; }
        public int OldPrice { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int NewPrice { get; set; }
        public bool inStock { get; set; }
        public int StockCount { get; set; }
        public bool isDeleted { get; set; }

        public List<IFormFile> ChildPhotos { get; set; }
        public string Color{ get; set; }
        public int Storage { get; set; }
        public IFormFile Photos { get; set; }
 

        public Nullable<DateTime> CreatedDate { get; set; }
        public class ProductCreateDtoValidatio : AbstractValidator<ProductCreateDto>
        {
            public ProductCreateDtoValidatio()
            {
                RuleFor(x => x.Title).NotEmpty().WithMessage("bosh qoyma").MaximumLength(10).WithMessage("10dan artiq ");
                RuleFor(x => x.NewPrice).GreaterThan(50).WithMessage("50 boyuk olmalidi");
                RuleFor(x => x.OldPrice).GreaterThan(50).WithMessage("50 boyuk olmalidi");
              
                RuleFor(p => p).Custom((p, context) =>
                {
                    if (p.NewPrice < p.OldPrice)
                    {
                        context.AddFailure("NewPrice", "NewPrice  OldPrice-dan  kicik ola bilmez");
                    }
                });
            }
        }
    }
}
