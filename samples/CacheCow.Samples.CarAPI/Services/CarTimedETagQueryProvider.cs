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
            throw new System.NotImplementedException();
        }
    }
}