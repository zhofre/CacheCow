﻿using AutoMapper;
using CacheCow.Samples.CarAPI.Helpers;
using CacheCow.Samples.CarAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CacheCow.Samples.CarAPI.Controllers
{
    [Route("api/cars")]
    public class CarsController : Controller
    {
        private readonly ICarRepository _repository;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public CarsController(
            ICarRepository repository,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor contextAccessor,
            IMapper mapper)
        {
            _repository = repository;
            _urlHelperFactory = urlHelperFactory;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetCars))]
        public IActionResult GetCars([FromQuery] RequestParameters requestParameters)
        {
            var itemsFromRepo = _repository.Get(requestParameters);
            
            dynamic paginationData = CreatePaginationData(
                nameof(GetCars),
                requestParameters,
                itemsFromRepo.TotalCount,
                UrlHelper);
            Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(paginationData);

            var dtos = Map(itemsFromRepo, requestParameters, paginationData.totalPages, UrlHelper);
            return Ok(dtos);
        }

        private IUrlHelper UrlHelper => _urlHelperFactory.GetUrlHelper(_contextAccessor.ActionContext);

        private static object CreatePaginationData(
            string requestName,
            RequestParameters parameters,
            int totalCount,
            IUrlHelper urlHelper)
        {
            var totalPages = Convert.ToInt32(Math.Ceiling(totalCount / (double)parameters.PageSize));
            var previousPageLink = parameters.PageNumber > 1
                ? parameters.CreateResourceUri(ResourceUriType.PreviousPage, requestName, urlHelper)
                : null;
            var nextPageLink = parameters.PageNumber < totalPages
                ? parameters.CreateResourceUri(ResourceUriType.NextPage, requestName, urlHelper)
                : null;

            var paginationMetadata = new
            {
                totalCount,
                pageSize = parameters.PageSize,
                currentPage = parameters.PageNumber,
                totalPages,
                previousPageLink,
                nextPageLink
            };

            return paginationMetadata;
        }

        private Dto.LinkedResourceCollection<Dto.Car> Map(
            IEnumerable<Entities.Car> entities,
            RequestParameters parameters,
            int totalPages,
            IUrlHelper urlHelper)
        {
            var dtoList = _mapper.Map<IEnumerable<Dto.Car>>(entities)
                .Select(x => AddLinks(x, urlHelper))
                .ToList();
            var result = new Dto.LinkedResourceCollection<Dto.Car>
            {
                Content = dtoList
            };
            return AddLinks(result, parameters, totalPages, urlHelper);
        }

        private Dto.Car Map(Entities.Car entity, IUrlHelper urlHelper)
        {
            return AddLinks(_mapper.Map<Dto.Car>(entity), urlHelper);
        }

        private Dto.LinkedResourceCollection<Dto.Car> AddLinks(
            Dto.LinkedResourceCollection<Dto.Car> cars,
            RequestParameters parameters,
            int totalPages,
            IUrlHelper urlHelper)
        {
            if (parameters.PageNumber > 1)
            {
                cars.Links.Add(new Dto.Link(
                    parameters.CreateResourceUri(
                        ResourceUriType.PreviousPage,
                        $"GetCars",
                        urlHelper),
                    "previousPage",
                    "GET"));
            }

            cars.Links.Add(new Dto.Link(
                parameters.CreateResourceUri(
                    ResourceUriType.CurrentPage,
                    $"GetCars",
                    urlHelper),
                "self",
                "GET"));

            if (parameters.PageNumber < totalPages)
            {
                cars.Links.Add(new Dto.Link(
                    parameters.CreateResourceUri(
                        ResourceUriType.NextPage,
                        $"GetCars",
                        urlHelper),
                    "nextPage",
                    "GET"));
            }

            return cars;
        }

        private Dto.Car AddLinks(
            Dto.Car car,
            IUrlHelper urlHelper)
        {
            return car;
        }
    }
}
