using AutoMapper;
using CacheCow.Samples.CarAPI.Helpers;
using CacheCow.Samples.CarAPI.Services;
using Microsoft.AspNetCore.Http;
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

            var urlHelper = CreateUrlHelper();
            dynamic paginationData = CreatePaginationData(
                nameof(GetCars),
                requestParameters,
                itemsFromRepo.TotalCount,
                urlHelper);
            Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(paginationData);

            var dtos = Map(itemsFromRepo, requestParameters, paginationData.totalPages, urlHelper);
            return Ok(dtos);
        }

        [HttpGet("{id}", Name = nameof(GetCar))]
        public IActionResult GetCar(int id)
        {
            var itemFromRepo = _repository.Get(id);
            if (itemFromRepo == null)
            {
                return NotFound();
            }

            var urlHelper = CreateUrlHelper();
            var dto = Map(itemFromRepo, urlHelper);
            return Ok(dto);
        }

        [HttpPost(Name = nameof(CreateCar))]
        public IActionResult CreateCar([FromBody] Dto.CarForCreation input)
        {
            if (input == null)
            {
                return BadRequest();
            }

            var entity = _mapper.Map<Entities.Car>(input);
            var savedEntity = _repository.Add(entity);

            var urlHelper = CreateUrlHelper();
            var dto = Map(savedEntity, urlHelper);
            return CreatedAtRoute(
                nameof(GetCar),
                new { id = entity.Id },
                dto);
        }
        
        [HttpPost("{id}")]
        public IActionResult BlockCarCreation(int id)
        {
            return _repository.Exists(id)
                ? new StatusCodeResult(StatusCodes.Status409Conflict)
                : NotFound();
        }

        [HttpPut("{id}", Name = nameof(UpdateCar))]
        public IActionResult UpdateCar(int id, [FromBody] Dto.CarForManipulation input)
        {
            if (input == null)
            {
                return BadRequest();
            }

            var entity = _repository.Get(id);
            if (entity == null)
            {
                return NotFound();
            }

            var entityForRepo = _mapper.Map(input, entity);
            _repository.Update(entityForRepo);

            return NoContent();
        }

        [HttpDelete("{id}", Name = nameof(DeleteCar))]
        public IActionResult DeleteCar(int id)
        {
            if(!_repository.Exists(id))
            {
                return NotFound();
            }

            _repository.Delete(id);

            return NoContent();
        }
        
        private IUrlHelper CreateUrlHelper() => _urlHelperFactory.GetUrlHelper(_contextAccessor.ActionContext);

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
                        nameof(GetCars),
                        urlHelper),
                    "previousPage",
                    "GET"));
            }

            cars.Links.Add(new Dto.Link(
                parameters.CreateResourceUri(
                    ResourceUriType.CurrentPage,
                    nameof(GetCars),
                    urlHelper),
                "self",
                "GET"));

            if (parameters.PageNumber < totalPages)
            {
                cars.Links.Add(new Dto.Link(
                    parameters.CreateResourceUri(
                        ResourceUriType.NextPage,
                        nameof(GetCars),
                        urlHelper),
                    "nextPage",
                    "GET"));
            }

            cars.Links.Add(new Dto.Link(
                    urlHelper.Link(nameof(CreateCar), null),
                    "create",
                    "POST"));

            return cars;
        }

        private Dto.Car AddLinks(
            Dto.Car car,
            IUrlHelper urlHelper)
        {
            car.Links.Add(new Dto.Link(
                urlHelper.Link(nameof(GetCar), new { id = car.Id }),
                "self",
                "GET"));
            car.Links.Add(new Dto.Link(
                    urlHelper.Link(nameof(UpdateCar), new { id = car.Id }),
                    "update",
                    "PUT"));
            car.Links.Add(new Dto.Link(
                    urlHelper.Link(nameof(DeleteCar), new { id = car.Id }),
                    "delete",
                    "DELETE"));
            return car;
        }
    }
}
