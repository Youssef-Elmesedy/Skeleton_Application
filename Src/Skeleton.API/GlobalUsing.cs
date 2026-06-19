// Global Usings — Web API Layer

global using MediatR;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Skeleton.Application.Common;
global using Skeleton.Responses;

// ── Category ──────────────────────────────────────────────────────
global using Skeleton.Application.Feature.Category.CategoryDto;
global using Skeleton.Application.Feature.Category.Commands.CreateCategory;
global using Skeleton.Application.Feature.Category.Commands.DeleteCategory;
global using Skeleton.Application.Feature.Category.Commands.UpdateCategory;
global using Skeleton.Application.Feature.Category.Queries.GetAll;
global using Skeleton.Application.Feature.Category.Queries.GetById;
global using Skeleton.Application.Feature.Category.Queries.GetPaged;
global using Skeleton.Application.Feature.Category.Queries.SearchCategory;

// ── Product ───────────────────────────────────────────────────────
global using Skeleton.Application.Feature.Product.Commands.CreateProduct;
global using Skeleton.Application.Feature.Product.Commands.DeleteProduct;
global using Skeleton.Application.Feature.Product.Commands.UpdateProduct;
global using Skeleton.Application.Feature.Product.ProductDto;
global using Skeleton.Application.Feature.Product.Queries.GetAll;
global using Skeleton.Application.Feature.Product.Queries.GetByCategory;
global using Skeleton.Application.Feature.Product.Queries.GetById;
global using Skeleton.Application.Feature.Product.Queries.GetPaged;
global using Skeleton.Application.Feature.Product.Queries.GetProductGroupByCategory;
global using Skeleton.Application.Feature.Product.Queries.Search;
