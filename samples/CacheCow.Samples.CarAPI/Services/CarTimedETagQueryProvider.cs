using System;
using System.Threading.Tasks;
using AutoMapper;
using CacheCow.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CacheCow.Samples.CarAPI.Services
{
    public class CarTimedETagQueryProvider : ITimedETagQueryProvider<Dto.Car>
    {
        private readonly ICarRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITimedETagExtractor<Dto.Car> _extractor;

        public CarTimedETagQueryProvider(
            ICarRepository repository,
            IMapper mapper,
            ITimedETagExtractor<Dto.Car> extractor)
        {
            _repository = repository;
            _mapper = mapper;
            _extractor = extractor;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        public Task<TimedEntityTagHeaderValue> QueryAsync(HttpContext context)
        {
            var routeData = context.GetRouteData();
            var path = context.Request.Path.ToString().ToLowerInvariant();

            Entities.Car car = null;
            if (path.EndsWith("last"))
            {
                car = _repository.Last();
            }
            else if(routeData.Values.ContainsKey("id"))
            {
                car = _repository.Get(Convert.ToInt32(routeData.Values["id"]));
            }
            else
            {
                throw new NotImplementedException();
            }

            var dto = _mapper.Map<Dto.Car>(car);
            return Task.FromResult(_extractor.Extract(dto));
        }
    }
}