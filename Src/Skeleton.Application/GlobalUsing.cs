// Global using directives for the Application layer

// Domain
global using Skeleton.Domain.Interfaces.InterfacesRepository;
global using Skeleton.Domain.Entities;
global using Skeleton.Domain.Exceptions;
global using Skeleton.Domain.BusinessReules;

// Application Infrastructure
global using Skeleton.Application.Interfaces.Queries;
global using Skeleton.Application.Common;
global using Skeleton.Application.Services.Interfaces;

// DTOs
global using Skeleton.Application.Feature.Product.ProductDto;

// Mapper
global using MapsterMapper;

// FluentValidation
global using FluentValidation;

// MediatR
global using MediatR;
global using Skeleton.Application.Behaviors;
